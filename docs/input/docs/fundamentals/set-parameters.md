---
Order: 40
Title: SetParameters Method
Description: Override the parameter values used by Cake.Recipe
---

Cake.Recipe is very customizable, in terms of how builds are executed, where files are stored, what notifications should be sent out, etc.  All of this customization is done through the SetParameters method, which gives you fine grain control over all aspects of Cake.Recipe.

As an example, you could do something like the following to override a number of the default parameter values (this example is taken from the Chocolatey GUI build):

```csharp
BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            solutionFilePath: "./Source/ChocolateyGui.sln",
                            solutionDirectoryPath: "./Source/ChocolateyGui",
                            resharperSettingsFileName: "ChocolateyGui.sln.DotSettings",
                            title: "Chocolatey GUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            appVeyorAccountName: "chocolatey",
                            appVeyorProjectSlug: "chocolateygui",
                            shouldDownloadFullReleaseNotes: true,
                            shouldDownloadMilestoneReleaseNotes: true,
                            shouldPublishChocolatey: false,
                            shouldPublishNuGet: false,
                            shouldPublishGitHub: false,
                            shouldRunGitVersion: true,
                            webLinkRoot: "ChocolateyGUI",
                            webBaseEditUrl: "https://github.com/chocolatey/ChocolateyGUI/tree/develop/docs/input/");
```

## Required Parameters

The SetParameters method has four input parameters which have to be provided, with the remainder being optional parameters that have default values.  The following is a list of the values that have to be provided.

### context

This is the current CakeContext, which can be provided directly as `Context`.

Type: `ICakeContext`

### buildSystem

This again if provided directly from Cake, and can be passed directly as `BuildSystem`.

Type: `BuildSystem`

### sourceDirectoryPath

This is the path to the directory where your source code files are stored.  Cake.Recipe is convention driven, and expects all the source files to be located in a folder like `./Source` or `./src` or similar.

Type: `DirectoryPath`

### title

This a essentially the name of the application that is being built within the repository.  For example, `Cake.Gem`, or `ChocolateyGUI`.  A number of the optional parameters will attempt to be derived from what you pass as the Title for your project, so depending on how you have named it, you might have to provide a value for some other parameters.

Type: `string`

## Optional Parameters

The SetParameters method uses the concept of optional parameters, in fact, all but four of the parameters to the SetParameters method are optional. To override a specific parameter, you need to use a named parameter. The following is a list of all the named parameters that can be used on the method.

### solutionFilePath

Cake.Recipe assumes that it is only building a single .Net solution file, and it needs to know the location of it.  It will attempt to form that location using the SourceDirectoryPath and the Title that are passed into SetParameters.

Type: `FilePath`

Default Value:

```csharp
SourceDirectoryPath.CombineWithFilePath(Title + ".sln")
```

### solutionDirectoryPath

This value is only used to feed into the default value for the DupFinderExcludePattern, which can be overridden in the SetToolSettings method.

Type: `DirectoryPath`

Default Value:

```csharp
SourceDirectoryPath.Combine(Title)
```

### rootDirectoryPath

This is intended to store the root directory path for the repository that Cake.Recipe is running in.  This is derived from the WorkingDirectory of Cake by default.

Type: `DirectoryPath`

Default Value:

```csharp
context.MakeAbsolute(context.Environment.WorkingDirectory)
```

### testDirectoryPath

This is the directory where any test projects for the solution are located.  By default, Cake.Recipe assumes that this is the same location as the normal projects within the solution.

Type: `DirectoryPath`

Default Value:

```csharp
sourceDirectoryPath
```

### testFilePattern

This is the pattern that is used to determine what files/projects should be run through unit testing.

Type: `string`

Default Value: `"/**/*Tests.dll"` or `"/**/*Tests.csproj"` depending on whether it is a .Net Framework or .Net Core execution.

### integrationTestScriptPath

Cake.Recipe has the concept of running a set of integration tests after the main portion of the build is finished.  This parameter stores the default location for the script that should be executed.

Type: `string`

Default Value:

```csharp
context.MakeAbsolute((FilePath)"test.cake");
```

### resharperSettingsFileName

This is the location of the settings file which should be passed to the InspectCode tool.

Type: `string`

Default Value:

```csharp
string.Format("{0}.sln.DotSettings", Title);
```

### repositoryOwner

GitHub repository names are typically in the format of `repositoryOwner/repositoryName`.  These values are used by the GitReleaseManager tool to identify which repository should be communicated with.

Type: `string`

Default Value:

```csharp
string.Empty
```

### repositoryName

GitHub repository names are typically in the format of `repositoryOwner/repositoryName`.  These values are used by the GitReleaseManager tool to identify which repository should be communicated with.

Type: `string`

Default Value:

```csharp
Title
```

### appVeyorAccountName

This value is used when executing the `Clear-AppVeyor-Cache` task.  This task is useful when you need to ensure that the AppVeyor build cache is cleared out properly before running a build.

Type: `string`

Default Value:

```csharp
RepositoryOwner.Replace("-", "").ToLower();
```

### appVeyorProjectSlug

This value is used when executing the `Clear-AppVeyor-Cache` task.  This task is useful when you need to ensure that the AppVeyor build cache is cleared out properly before running a build.

Type: `string`

Default Value:

```csharp
Title.Replace(".", "-").ToLower();
```

### shouldPostToGitter

This is used as a final control variable for whether or not notification messages should be posted to Gitter when the a final release build (i.e. a tagged build) completes.

Type: `bool`

Default Value:

```csharp
true
```

### shouldPostToSlack

This is used as a final control variable for whether or not notification messages should be posted to Slack when a build fails.

Type: `bool`

Default Value:

```csharp
true
```

### shouldPostToTwitter

This is used as a final control variable for whether or not notification messages should be posted to Twitter when the a final release build (i.e. a tagged build) completes.

Type: `bool`

Default Value:

```csharp
true
```

### shouldPostToMicrosoftTeams

This is used as a final control variable for whether or not notification messages should be posted to Microsoft Teams when the a final release build (i.e. a tagged build) completes.

Type: `bool`

Default Value:

```csharp
false
```

### shouldSendEmail

This is used as a final control variable for whether or not notification messages should be sent via email when the a final release build (i.e. a tagged build) completes, or when a build fails.

Type: `bool`

Default Value:

```csharp
true
```

### shouldDownloadMilestoneReleaseNotes

When executing the `Export-Release-Notes` task, this variable controls whether or not the release notes for the current milestone is exported by GitReleaseManager.

Type: `bool`

Default Value:

```csharp
false
```

### shouldDownloadFullReleaseNotes

When executing the `Export-Release-Notes` task, this variable controls whether or not the all release notes are exported by GitReleaseManager.

Type: `bool`

Default Value:

```csharp
false
```

### shouldNotifyBetaReleases

At the end of a build, Cake.Recipe determines what branch the build is executing on, and decides whether a notification should be sent out or not.  If the branch in question is a hotfix or a release branch, notifications will only be sent if this parameter is set to true.

Type: `bool`

Default Value:

```csharp
false
```

### shouldDeleteCachedFiles

At the end of a build, Cake.Recipe decides whether or not cached files, such as nupkg files, are deleted or not.  This parameter controls whether this should happen or not, in additional to _where_ the build is happening, i.e. locally or on a CI/CD platform.

Type: `bool`

Default Value:

```csharp
false
```

### shouldUseDeterministicBuilds

If this parameter is set to true, then during the execution of either MSBuild or the .Net Core CLI, a build property called `ContinuousIntegrationBuild` with the value of `true` will be added.

Type: `bool`

Default Value:

```csharp
true
```

### milestoneReleaseNotesFilePath

This is the location of the file that milestone release notes are exported to by GitReleaseManager when running the `Export-Release-Notes` task.

Type: `FilePath`

Default Value:

```csharp
RootDirectoryPath.CombineWithFilePath("CHANGELOG.md");
```

### fullReleaseNotesFilePath

This is the location of the file that the full release notes are exported to by GitReleaseManager when running the `Export-Release-Notes` task.

Type: `FilePath`

Default Value:

```csharp
RootDirectoryPath.CombineWithFilePath("ReleaseNotes.md");
```

### shouldRunChocolatey

This is used as a final control variable for whether or not the Chocolatey tool should be executed as part of the build.

Type: `bool`

Default Value:

```csharp
true
```

### shouldPublishGitHub

This is used as a final control variable for whether or not a final set of artifacts from the build should be published to GitHub, via the GitReleaseManager tool.

Type: `bool`

Default Value:

```csharp
true
```

### shouldGenerateDocumentation

This is used as a final control variable for whether or not Wyam should be executed to generate documentation for the project.

Type: `bool`

Default Value:

```csharp
true
```

### shouldDocumentSourceFiles

This is used as a final control variable for whether or not Wyam should be executed against the source files for the project to generate additional documentation.

Type: `bool`

Default Value:

```csharp
true
```

### shouldRunDupFinder

This is used as a final control variable for whether or not the DupFinder tool should be executed as part of the build.

Type: `bool`

Default Value:

```csharp
true
```

### shouldRunInspectCode

This is used as a final control variable for whether or not the InspectCode tool should be executed as part of the build.

Type: `bool`

Default Value:

```csharp
true
```

### shouldRunCoveralls

This is used as a final control variable for whether or not unit test coverage reports should be uploaded to Coveralls as part of the build.

Type: `bool`

Default Value:

```csharp
true
```

### shouldRunCodecov

This is used as a final control variable for whether or not unit test coverage reports should be uploaded to CodeCov as part of the build.

Type: `bool`

Default Value:

```csharp
true
```

### shouldRunDotNetCorePack

By default, Cake.Recipe assumes that any NuGet packages are generated via the presence of nuspec files in correct folder.  However, it is possible to force the execution of .Net Core pack by setting this parameter to true.

Type: `bool`

Default Value:

```csharp
false
```

### shouldBuildNugetSourcePackage

When this is set to true, a number of different settings are enabled, for example, during the execution of the .Net Core CLI or NuGet, a build property called
`SymbolPackageFormat` is set to `true`, and also the DotNetCorePackSettings properties IncludeSource and IncludeSymbols are set to true, and finally the NuGetPackSettings property Symbols is set to true.

Type: `bool`

Default Value:

```csharp
false
```

### shouldRunIntegrationTests

This is used as a final control variable for whether or not integration tests are executed as part of the Cake.Recipe build.

Type: `bool`

Default Value:

```csharp
false
```

### shouldCalculateVersion

This is used as a final control variable for whether or not a version number for the repository is asserted as part of the build.  By default, the GitVersion tool is used to calculate this version number.

Type: `bool`

Default Value:

```csharp
true
```

### shouldUseTargetFrameworkPath

This is used as a final control variable for whether or not the Target Framework Path should be used.

Type: `bool?`

Default Value:

```csharp
!context.IsRunningOnWindows();
```

### transifexEnabled

This is the final control variable for whether or not the Transifex tool should be executed.  The default value is calculated based on the presence of this file in the repository `./.tx/config`.

Type: `bool?`

Default Value:

```csharp
TransifexIsConfiguredForRepository(context)
```

### transifexPullMode

This controls what type of translation files are downloaded when executing the Transifex tool.

Type: `TransifexMode`

Default Value:

```csharp
TransifexMode.OnlyTranslated
```

### transifexPullPercentage

When running the Transifex tool, this parameter controls the percentage completion level for a translation file, to know whether it should be downloaded or not.

Type: `int`

Default Value:

```csharp
60
```

### gitterMessage

This is the message that is sent to Gitter at the end of a tagged build.  This is formatted with the calculated version number, as well as the Title parameter.

Type: `string`

Default Value:

```csharp
@/all Version {0} of the {1} Addin has just been released, this will be available here https://www.nuget.org/packages/{1}, once package indexing is complete."
```

### microsoftTeamsMessage

This is the message that is sent to Microsoft Teams at the end of a tagged build.  This is formatted with the calculated version number, as well as the Title parameter.

Type: `string`

Default Value:

```csharp
Version {0} of the {1} Addin has just been released, this will be available here https://www.nuget.org/packages/{1}, once package indexing is complete."
```

### twitterMessage

This is the message that is sent to Twitter at the end of a tagged build.  This is formatted with the calculated version number, as well as the Title parameter.

Type: `string`

Default Value:

```csharp
Version {0} of the {1} Addin has just been released, this will be available here https://www.nuget.org/packages/{1}, once package indexing is complete."
```

### wyamRootDirectoryPath

This is the directory that stores the documentation files that will be passed to the Wyam tool.

Type: `DirectoryPath`

Default Value:

```csharp
context.MakeAbsolute(context.Directory("docs"));
```

### wyamPublishDirectoryPath

This is the directory that will be passed to Wyam for where the output documentation should be published to.

Type: `DirectoryPath`

Default Value:

```csharp
context.MakeAbsolute(context.Directory("BuildArtifacts/temp/_PublishedDocumentation"));
```

### wyamConfigurationFile

This is the location of the configuration file that will be passed to the Wyam tool.

Type: `FilePath`

Default Value:

```csharp
context.MakeAbsolute((FilePath)"config.wyam")
```

### wyamRecipe

This is the name of the recipe that should be used by the Wyam tool.

Type: `string`

Default Value:

```csharp
Docs
```

### wyamTheme

This is the name of the them that should be used by the Wyam tool.
Type: `string`

Default Value:

```csharp
Samson
```

### wyamSourceFiles

This is the location of all the source files that should be used to generate documentation by the Wyam tool.

Type: `string`

Default Value:

```csharp
"../../" + SourceDirectoryPath.FullPath + "/**/{!bin,!obj,!packages,!*.Tests,}/**/*.cs";
```

### webHost

The root URL for generating documentation via the Wyam tool.

Type: `string`

Default Value:

```csharp
string.Format("{0}.github.io", repositoryOwner)
```

### webLinkRoot

A value which is concatenated to the WebLinkRoot when generating documentation via the Wyam tool.

Type: `string`

Default Value:

```csharp
RepositoryName
```

### webBaseEditUrl

When generating documentation via Wyam, this value is used to create a clickable link for editing the documentation when published.

Type: `string`

Default Value:

```csharp
string.Format("https://github.com/{0}/{1}/tree/{2}/docs/input/", repositoryOwner, title, developBranchName)
```

### nuspecFilePath

When running the top level RunNuGet task, this parameter value is used to locate the nuspec file which should be packed.

Type: `FilePath`

Default Value:

```csharp
context.MakeAbsolute((FilePath)"./Cake.Recipe/Cake.Recipe.nuspec")
```

### isPublicRepository

This parameter is used in the execution of GitVersion to determine whether actions should be taken.

Type: `bool`

Default Value:

```csharp
true
```

### nugetConfig

When this file exists, it is used to populate the NuGetSources parameter, which is ultimately used when executing nuget or dotnet restore.

Type: `FilePath`

Default Value:

```csharp
(FilePath)"./NuGet.Config");
```

### nuGetSources

These are the sources used when executing nuget or dotnet restore.

Type: `ICollection<string>`

Default Value:

If a NuGet.Config file isn't located, the following URLs are used.

```csharp
new []{
    "https://api.nuget.org/v3/index.json",
    "https://www.myget.org/F/cake-contrib/api/v3/index.json"
};
```

### treatWarningsAsErrors

This parameter is passed into the execution of MSBuild.

Type: `bool`

Default Value:

```csharp
true
```

### masterBranchName

The name of the main branch used in the repository.  This is used during the execution of GitReleaseManager.

Type: `string`

Default Value:

```csharp
master
```

### developBranchName

The name of the development branch used in the repository.  This is used in the generation of other parameters.

Type: `string`

Default Value:

```csharp
develop
```

### emailRecipient

The address of the person receiving the email.  Used when sending notification emails at the end of a build.

Type: `string`

Default Value:

```csharp
null
```

### emailSenderName

The name of the person sending the email.  Used when sending notification emails at the end of a build.

Type: `string`

Default Value:

```csharp
null
```

### emailSenderAddress

The email address of the person sending the email.  Used when sending notification emails at the end of a build.

Type: `string`

Default Value:

```csharp
null
```

### restorePackagesDirectory

This is the directory passed to dotnet restore, to control where packages are restored to.

Type: `DirectoryPath`

Default Value:

```csharp
null
```

### packageSourceDatas

This is a list of both NuGet and Chocolatey sources that should be used when publishing via the NuGet, dotnet, or Chocolatey tools.  This includes the concept of both prerelease and release sources.

By default, Cake.Recipe attempts to locate sources using the presence of deprecated environment variables on the system.

In addition, it adds sources for both Azure Packages and GitHub Package Repository.

These sources can be configured to either use UserName/Password combination, or an API Key.

Type: `List<PackageSourceData>`

Default Value:

```csharp
PackageSources = new List<PackageSourceData>();

// Try to get the deprecated way of doing things, set them as default sources
var myGetUrl = context.EnvironmentVariable("MYGET_SOURCE");
if (!string.IsNullOrEmpty(myGetUrl))
{
    PackageSources.Add(new PackageSourceData(context, "MYGET", myGetUrl, FeedType.NuGet, false));
    PackageSources.Add(new PackageSourceData(context, "MYGET", myGetUrl, FeedType.Chocolatey, false));
}

var nuGetUrl = context.EnvironmentVariable("NUGET_SOURCE");
if (!string.IsNullOrEmpty(nuGetUrl))
{
    PackageSources.Add(new PackageSourceData(context, "NUGET", nuGetUrl));
}

var chocolateyUrl = context.EnvironmentVariable("CHOCOLATEY_SOURCE");
if (!string.IsNullOrEmpty(chocolateyUrl))
{
    PackageSources.Add(new PackageSourceData(context, "CHOCOLATEY", chocolateyUrl, FeedType.Chocolatey));
}

// The following aren't deprecated sources, but rather suggested defaults going forward, so check
// for the environment variables being set, if they are, add them to the list
var azureUrl = context.EnvironmentVariable("AZURE_SOURCE");
if (!string.IsNullOrEmpty(azureUrl))
{
    PackageSources.Add(new PackageSourceData(context, "AZURE", azureUrl, FeedType.NuGet, false));
}

var gprUrl = context.EnvironmentVariable("GPR_SOURCE");
if(!string.IsNullOrEmpty(gprUrl))
{
    PackageSources.Add(new PackageSourceData(context, "GPR", gprUrl, FeedType.NuGet, false));
}
```

### preferredBuildAgentOperatingSystem

It is possible to run Cake.Recipe on multiple CI/CD platforms and operating systems for the same project.  When doing this, it is essential to be able to specify what combination of CI/CD and operating system is the preferred one, so that artifacts are only published once.

Type: `PlatformFamily`

Default Value:

```csharp
PlatformFamily.Windows
```

### preferredBuildProviderType

It is possible to run Cake.Recipe on multiple CI/CD platforms and operating systems for the same project.  When doing this, it is essential to be able to specify what combination of CI/CD and operating system is the preferred one, so that artifacts are only published once.

Type: `BuildProviderType`

Default Value:

```csharp
BuildProviderType.AppVeyor
```
