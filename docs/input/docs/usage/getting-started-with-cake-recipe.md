---
Order: 5
Title: Getting started with Cake.Recipe
Description: How to get started
---

This guide will show you how to setup Cake.Recipe for a new project.

# 1. Download the files

You can retrieve the Cake.Recipe files from [GitHub](https://github.com/cake-contrib/Cake.Recipe) or the entire package from [Nuget](https://www.nuget.org/packages/Cake.Recipe).
The only two files you'll need straight away are the `build.ps1` and `recipe.cake` files. Put these in the root of your repository.

The `build.ps1` is a modified version of the standard cake bootstrapper which defaults to using recipe.cake instead of build.cake.

The `recipe.cake` file will load the recipes and run the tasks when the build script is run.

# 2. Configure the `recipe.cake` file for your project

Change the default values in the `recipe.cake` to suit your project.
The sourceDirectoryPath should be the path to the folder containing your solution file.
There are other properties that can be set in this method that will affect which tasks run and how they behave during the build.
Look at the `parameters.cake` file to see the complete list of parameters.

```csharp
BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "Cake.Recipe",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Recipe",
                            appVeyorAccountName: "cakecontrib");
```

# 3. Run the Cake.Recipe build

Now you should be able to run your Cake.Recipe build by invoking the bootstrapper.
This step is the same as that described on Step 3 on the [Setting up a new project](https://www.cakebuild.net/docs/tutorials/setting-up-a-new-project) page.

# 4. Getting the most from Cake.Recipe

Cake.Recipe has a wide variety of built in tasks, many of which need additional configuration to work correctly.
This configuration is either set with parameters in the `BuildParameters.SetParameters` method, with environment variables, or through custom configuration files such as `.AppVeyor.yml`.
For more details on setting environment variables visit the [Cake.Recipe documentation](../fundamentals/environment-variables).

More information on how to configure various tasks can be found in [usage](./) section on this site

# 5. Modifying the Cake.Recipe build

Cake.Recipe will not suit everyone out of the box, but it has been designed in an [extensible](../extending) way so you can still load your own cake scripts, or you can modify the tasks that come with Cake.Recipe.

# 6. Next steps

Try configuring the additional tasks by including configuration files for the tools that have been added, i.e. AppVeyor, GitVersion, Wyam, etc, and be sure to look around the [Cake.Recipe](../) site for more information.
