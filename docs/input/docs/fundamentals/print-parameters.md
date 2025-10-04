---
Order: 50
Title: PrintParameters Method
Description: Prints the values of all parameters used by Cake.Recipe
---

Due to the extensible nature of Cake.Recipe, with various different parameter values, it can be useful to be able to see what the current value is for each parameter during the build process.

The can be achieved by adding the following to the [recipe.cake](./recipe-cake) file:

```csharp
BuildParameters.PrintParameters(Context);
```

The only required parameter is an `ICakeContext` instance.

Here is an example of the output of this method taken from the [GitReleaseManager](https://github.com/gittools/gitreleasemanager) project.

```bash
Printing Build Parameters...
IsLocalBuild: False
IsPullRequest: True
IsMainRepository: True
IsPublicRepository: True
IsTagged: False
BranchType: Develop
TreatWarningsAsErrors: True
ShouldSendEmail: True
ShouldPostToSlack: True
ShouldPostToTwitter: True
ShouldPostToMicrosoftTeams: False
ShouldDownloadFullReleaseNotes: False
ShouldDownloadMilestoneReleaseNotes: False
ShouldNotifyBetaReleases: False
ShouldDeleteCachedFiles: False
ShouldGenerateDocumentation: False
ShouldDocumentSourceFiles: False
ShouldRunIntegrationTests: True
ShouldCalculateVersion: True
BuildAgentOperatingSystem: Windows
IsRunningOnAppVeyor: True
RepositoryOwner: GitTools
RepositoryName: GitReleaseManager
TransifexEnabled: False
CanPullTranslations: False
CanPushTranslations: False
PrepareLocalRelease: False
BuildAgentOperatingSystem: Windows
ForceContinuousIntegration: False
WyamRootDirectoryPath: C:/projects/gitreleasemanager/docs
WyamPublishDirectoryPath: C:/projects/gitreleasemanager/BuildArtifacts/temp/_PublishedDocumentation
WyamConfigurationFile: C:/projects/gitreleasemanager/config.wyam
WyamRecipe: Docs
WyamTheme: Samson
Wyam Deploy Branch: gh-pages
Wyam Deploy Remote: https://github.com/GitTools/GitReleaseManager
WebHost: GitTools.github.io
WebLinkRoot: GitReleaseManager
WebBaseEditUrl: https://github.com/GitTools/GitReleaseManager/tree/develop/docs/input/
NuSpecFilePath: C:/projects/gitreleasemanager/Cake.Recipe/Cake.Recipe.nuspec
NugetConfig: C:/projects/gitreleasemanager/NuGet.Config (False)
NuGetSources: https://api.nuget.org/v3/index.json, https://www.myget.org/F/cake-contrib/api/v3/index.json
RestorePackagesDirectory: [NULL]
EmailRecipient: [NULL]
EmailSenderName: [NULL]
EmailSenderAddress: [NULL]
Setting up tools...

```
