---
Order: 15
Title: How to create a NuGet or Chocolatey Package
---

This document describes how packaging works when using the `Cake.Recipe` script.

## Initial Configuration

You have to perform this initial configuration that it drives everything.  

## Environment Variables

If an environment variable that is defined [here](https://cake-contrib.github.io/Cake.Recipe/docs/fundamentals/environment-variables) is set, then the corresponding task that would use that will be executed. Pretty nifty and may be something that is easily missed.

So for example, if I want to start publishing a MyGet package, then I would need to set the following on my build server.

```
MYGET_API_KEY = <your key>
MYGET_SOURCE = <your source>
```

## Creating Packages

Creating packages, either Chocolatey or NuGet, follows another set of conventions. It requires that you have those environment variables configured and that you have a specific folder structure with your NuSpec files located in those.  In the [paths.cake](https://github.com/cake-contrib/Cake.Recipe/blob/develop/Cake.Recipe/Content/paths.cake) file, there is a class called *BuildPaths* that configures all of your build paths based on the initial project configuration that that needs to occur. In that class is the following snippet.

```
var nugetNuspecDirectory = "./nuspec/nuget";
var chocolateyNuspecDirectory = "./nuspec/chocolatey";
```

The snippet above is looking for a *nuspec* directory at the root of your project. Inside of that directory it is looking for *nuget* and *chocolatey* directories. If those directories do not exist and/or there isn't any nuspec files located then the packaging task for that package type is skipped. Any *nuspec* files found in either directory will be harvested and a package will be generated and pushed to central package directory based on package type. 

## How it works

With that added, it will push that to MyGet. I am going to guess that you may now be wondering how does that work. If you go to the [parameters.cake](https://github.com/cake-contrib/Cake.Recipe/blob/develop/Cake.Recipe/Content/parameters.cake) file in the repository you will notice that there is a static class called *BuildParameters*, it has a property called *ShouldPublishMyGet*. That is configured to be *true* by default, however, there is a check in that class that determines if it should be true or false.

```
shouldPublishMyGet = (!IsLocalBuild &&
                        !IsPullRequest &&
                        IsMainRepository &&
                        (IsTagged || BuildParameters.BranchType != BranchType.Master) &&
                        shouldPublishMyGet);
```

Above, you can tell that it cannot be local build, pull request, not the master branch, and that it should publish to MyGet. Now the last piece of the puzzle is located in the [nuget.cake](https://github.com/cake-contrib/Cake.Recipe/blob/develop/Cake.Recipe/Content/nuget.cake) file. That is the Cake file that contains the tasks that create and publish packages. The *Publish-MyGet-Packages* task has two criteria, that you want to publish a MyGet package and that packages have actually been created. This task also contains the check for the environment variables we set above and will record that error and continue to execute the next task.

Finally, the packages from each package directory will be harvested and all packages found will be pushed to their respective feeds.