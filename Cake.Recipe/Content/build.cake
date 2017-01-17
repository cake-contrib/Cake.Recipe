///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Starting Setup...");

    if(BuildParameters.IsMasterBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    BuildParameters.SetBuildVersion(
        BuildVersion.CalculatingSemanticVersion(
            context: Context
        )
    );

    Information("Building version {0} of " + BuildParameters.Title + " ({1}, {2}) using version {3} of Cake. (IsTagged: {4})",
        BuildParameters.Version.SemVersion,
        BuildParameters.Configuration,
        BuildParameters.Target,
        BuildParameters.Version.CakeVersion,
        BuildParameters.IsTagged);
});

Teardown(context =>
{
    Information("Starting Teardown...");

    if(context.Successful)
    {
        if(!BuildParameters.IsLocalBuild && !BuildParameters.IsPullRequest && BuildParameters.IsMainRepository && BuildParameters.IsMasterBranch && BuildParameters.IsTagged)
        {
            var message = "Version " + BuildParameters.Version.SemVersion + " of " + BuildParameters.Title + " Addin has just been released, https://www.nuget.org/packages/" + BuildParameters.Title + ".";

            if(BuildParameters.CanPostToTwitter && BuildParameters.ShouldPostToTwitter)
            {
                SendMessageToTwitter(message);
            }

            if(BuildParameters.CanPostToGitter && BuildParameters.ShouldPostToGitter)
            {
                SendMessageToGitterRoom("@/all Version " + BuildParameters.Version.SemVersion + " of the " + BuildParameters.Title + " Addin has just been released, https://www.nuget.org/packages/" + BuildParameters.Title + ".");
            }

            if(BuildParameters.CanPostToMicrosoftTeams && BuildParameters.ShouldPostToMicrosoftTeams)
            {
                SendMessageToMicrosoftTeams(message);
            }
        }
    }
    else
    {
        if(!BuildParameters.IsLocalBuild && BuildParameters.IsMainRepository)
        {
            if(BuildParameters.CanPostToSlack && BuildParameters.ShouldPostToSlack)
            {
                SendMessageToSlackChannel("Continuous Integration Build of " + BuildParameters.Title + " just failed :-(");
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
    Information("Target: {0}", BuildParameters.Target);
    Information("Configuration: {0}", BuildParameters.Configuration);

    Information("Solution FilePath: {0}", MakeAbsolute((FilePath)BuildParameters.SolutionFilePath));
    Information("Solution DirectoryPath: {0}", MakeAbsolute((DirectoryPath)BuildParameters.SolutionDirectoryPath));
    Information("Source DirectoryPath: {0}", MakeAbsolute(BuildParameters.SourceDirectoryPath));
    Information("Build DirectoryPath: {0}", MakeAbsolute(BuildParameters.Paths.Directories.Build));
});

Task("Clean")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
});

Task("Restore")
    .Does(() =>
{
    Information("Restoring {0}...", BuildParameters.SolutionFilePath);

    NuGetRestore(BuildParameters.SolutionFilePath, new NuGetRestoreSettings { Source = new List<string> { "https://www.nuget.org/api/v2", "https://www.myget.org/F/gep13/api/v2" }});
});

Task("Build")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-AppVeyor-Environment-Variables")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Building {0}", BuildParameters.SolutionFilePath);

    var msbuildSettings = new MSBuildSettings().
            SetPlatformTarget(ToolSettings.BuildPlatformTarget)
            .WithProperty("TreatWarningsAsErrors","true")
            .WithTarget("Build")
            .SetConfiguration(BuildParameters.Configuration);

    if(ToolSettings.OutputDirectory != null)
    {
        msbuildSettings.WithProperty("OutDir", MakeAbsolute(ToolSettings.OutputDirectory).FullPath);
    }

    // TODO: Need to have an XBuild step here as well
    MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);;
});

Task("Package")
    .IsDependentOn("Export-Release-Notes")
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
    .IsDependentOn("Publish-Documentation")
    .Finally(() =>
{
    if(publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + BuildParameters.Title + ".  All publishing tasks have been attempted.");
    }
});

Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

Task("ClearCache")
  .IsDependentOn("Clear-AppVeyor-Cache");

Task("Preview")
  .IsDependentOn("Preview-Documentation");
  
///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

public Builder Build
{
    get
    {
        return new Builder(target => RunTarget(target));
    }
}

public class Builder
{
    private Action<string> _action;

    public Builder(Action<string> action)
    {
        _action = action;
    }

    public void Run()
    {
        _action(BuildParameters.Target);       
    }
}