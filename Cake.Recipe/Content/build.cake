///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var parameters = BuildParameters.GetParameters(Context, BuildSystem, repositoryOwner, repositoryName);
var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    if(parameters.IsMasterBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    parameters.SetBuildPaths(
        BuildPaths.GetPaths(sourceDirectoryPath,
            context: Context
        )
    );

    parameters.SetBuildVersion(
        BuildVersion.CalculatingSemanticVersion(
            context: Context,
            parameters: parameters
        )
    );

    Information("Building version {0} of " + title + " ({1}, {2}) using version {3} of Cake. (IsTagged: {4})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target,
        parameters.Version.CakeVersion,
        parameters.IsTagged);
});

Teardown(context =>
{
    if(context.Successful)
    {
        if(!parameters.IsLocalBuild && !parameters.IsPullRequest && parameters.IsMainRepository && parameters.IsMasterBranch && parameters.IsTagged)
        {
            if(sendMessageToTwitter)
            {
                SendMessageToTwitter("Version " + parameters.Version.SemVersion + " of " + title + " Addin has just been released, https://www.nuget.org/packages/" + title + ".");
            }

            if(sendMessageToGitterRoom)
            {
                SendMessageToGitterRoom("@/all Version " + parameters.Version.SemVersion + " of the " + title + " Addin has just been released, https://www.nuget.org/packages/" + title + ".");
            }
        }
    }
    else
    {
        if(!parameters.IsLocalBuild && parameters.IsMainRepository)
        {
            if(sendMessageToSlackChannel)
            {
                SendMessageToSlackChannel("Continuous Integration Build of " + title + " just failed :-(");
            }
        }
    }

    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Show-Info")
    .Does(() =>
{
    Information("Target: {0}", parameters.Target);
    Information("Configuration: {0}", parameters.Configuration);

    Information("Solution FilePath: {0}", MakeAbsolute((FilePath)solutionFilePath));
    Information("Solution DirectoryPath: {0}", MakeAbsolute((DirectoryPath)solutionDirectoryPath));
    Information("Source DirectoryPath: {0}", MakeAbsolute(parameters.Paths.Directories.Source));
    Information("Build DirectoryPath: {0}", MakeAbsolute(parameters.Paths.Directories.Build));
});

Task("Clean")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(parameters.Paths.Directories.ToClean);
});

Task("Restore")
    .Does(() =>
{
    Information("Restoring {0}...", solutionFilePath);

    NuGetRestore(solutionFilePath, new NuGetRestoreSettings { Source = new List<string> { "https://www.nuget.org/api/v2", "https://www.myget.org/F/gep13/api/v2" }});
});

Task("Build")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-AppVeyor-Environment-Variables")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Building {0}", solutionFilePath);

    // TODO: Need to have an XBuild step here as well
    MSBuild(solutionFilePath, settings =>
        settings.SetPlatformTarget(PlatformTarget.MSIL)
            .WithProperty("TreatWarningsAsErrors","true")
            .WithProperty("OutDir", MakeAbsolute(parameters.Paths.Directories.TempBuild).FullPath)
            .WithTarget("Build")
            .SetConfiguration(parameters.Configuration));
});

Task("Package")
    .IsDependentOn("Create-NuGet-Packages")
    .IsDependentOn("Create-Chocolatey-Packages")
    .IsDependentOn("Test")
    .IsDependentOn("Analyze");

Task("Default")
    .IsDependentOn("Package");

Task("AppVeyor")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Upload-Coverage-Report")
    .IsDependentOn("Publish-MyGet-Packages")
    .IsDependentOn("Publish-Chocolatey-Packages")
    .IsDependentOn("Publish-Nuget-Packages")
    .IsDependentOn("Publish-GitHub-Release")
    .Finally(() =>
{
    if(publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + title + ".  All publishing tasks have been attempted.");
    }
});

Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

Task("ClearCache")
  .IsDependentOn("Clear-AppVeyor-Cache");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(parameters.Target);