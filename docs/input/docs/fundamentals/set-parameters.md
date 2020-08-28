---
Order: 40
Title: SetParameters Method
Description: Override the parameter values used by Cake.Recipe
---

## Required Parameters

### context

Type: `ICakeContext`

### buildSystem

Type: `BuildSystem`

### sourceDirectoryPath

Type: `DirectoryPath`

### title

Type: `string`

## Optional Parameters

### solutionFilePath

Type: `FilePath`

### solutionDirectoryPath

Type: `DirectoryPath`

### rootDirectoryPath

Type: `DirectoryPath`

### testDirectoryPath

Type: `DirectoryPath`

### testFilePattern

Type: `string`

### integrationTestScriptPath

Type: `string`

### resharperSettingsFileName

Type: `string`

### repositoryOwner

Type: `string`

### repositoryName

Type: `string`

### appVeyorAccountName

Type: `string`

### appVeyorProjectSlug

Type: `string`

### shouldPostToGitter

Type: `bool`

### shouldPostToSlack

Type: `bool`

### shouldPostToTwitter

Type: `bool`

### shouldPostToMicrosoftTeams

Type: `bool`

### shouldSendEmail

Type: `bool`

### shouldDownloadMilestoneReleaseNotes

Type: `bool`

### shouldDownloadFullReleaseNotes

Type: `bool`

### shouldNotifyBetaReleases

Type: `bool`

### shouldDeleteCachedFiles

Type: `bool`

### shouldUseDeterministicBuilds

Type: `bool`

### milestoneReleaseNotesFilePath

Type: `FilePath`

### fullReleaseNotesFilePath

Type: `FilePath`

### shouldRunChocolatey

Type: `bool`

### shouldPublishGitHub

Type: `bool`

### shouldGenerateDocumentation

Type: `bool`

### shouldDocumentSourceFiles

Type: `bool`

### shouldRunDupFinder

Type: `bool`

### shouldRunInspectCode

Type: `bool`

### shouldRunCoveralls

Type: `bool`

### shouldRunCodecov

Type: `bool`

### shouldRunDotNetCorePack

Type: `bool`

### shouldBuildNugetSourcePackage

Type: `bool`

### shouldRunIntegrationTests

Type: `bool`

### shouldCalculateVersion

Type: `bool`

### shouldUseTargetFrameworkPath

Type: `bool?`

### transifexEnabled

Type: `bool?`

### transifexPullMode

Type: `TransifexMode`

### transifexPullPercentage

Type: `int`

### gitterMessage

Type: `string`

### microsoftTeamsMessage

Type: `string`

### twitterMessage

Type: `string`

### wyamRootDirectoryPath

Type: `DirectoryPath`

### wyamPublishDirectoryPath

Type: `DirectoryPath`

### wyamConfigurationFile

Type: `FilePath`

### wyamRecipe

Type: `string`

### wyamTheme

Type: `string`

### wyamSourceFiles

Type: `string`

### webHost

Type: `string`

### webLinkRoot

Type: `string`

### webBaseEditUrl

Type: `string`

### nuspecFilePath

Type: `FilePath`

### isPublicRepository

Type: `bool`

### nugetConfig

Type: `FilePath`

### nuGetSources

Type: `ICollection<string>`

### treatWarningsAsErrors

Type: `bool`

### masterBranchName

Type: `string`

### developBranchName

Type: `string`

### emailRecipient

Type: `string`

### emailSenderName

Type: `string`

### emailSenderAddress

Type: `string`

### shouldPublishToMyGetWithApiKey

Type: `bool`

### restorePackagesDirectory

Type: `DirectoryPath`

### packageSourceDatas

Type: `List<PackageSourceData>`

### preferredBuildAgentOperatingSystem

Type: `PlatformFamily`

### preferredBuildProviderType

Type: `BuildProviderType`