///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information(Figlet(BuildParameters.Title));

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

    NuGetRestore(
        BuildParameters.SolutionFilePath,
        new NuGetRestoreSettings 
        { 
            Source = new List<string> 
            { 
                "https://www.nuget.org/api/v2",
                "https://www.myget.org/F/gep13/api/v2",
                BuildParameters.MyGet.SourceUrl 
            }
        });
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
            .SetMaxCpuCount(0)
            .SetConfiguration(BuildParameters.Configuration)
            .WithLogger(
                Context.Tools.Resolve("MSBuild.ExtensionPack.Loggers.dll").FullPath,
                "XmlFileLogger",
                string.Format(
                    "logfile=\"{0}\";invalidCharReplacement=_;verbosity=Detailed;encoding=UTF-8",
                    BuildParameters.Paths.Files.BuildLogFilePath)
            );


    // TODO: Need to have an XBuild step here as well
    MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);;

    if(BuildParameters.ShouldExecuteGitLink)
    {
        ExecuteGitLink();
    }

    CopyBuildOutput();
});

public void CopyBuildOutput()
{
    Information("Copying build output...");

    foreach(var project in ParseSolution(BuildParameters.SolutionFilePath).Projects)
    {
        // There is quite a bit of duplication in this function, that really needs to be tidied Upload

        // If the project is a solution folder, move along, as there is nothing to be done here
        if(project.IsSolutionFolder())
        {
            Information("Project is Solution Folder, so nothing to do here.");
            continue;
        }

        var parsedProject = ParseProject(project.Path, BuildParameters.Configuration);

        if(project.Path.FullPath.ToLower().Contains("wixproj"))
        {
            Warning("Skipping wix project");
            continue;
        }

        if(parsedProject.OutputPath == null || parsedProject.RootNameSpace == null || parsedProject.OutputType == null)
        {
            throw new Exception(string.Format("Unable to parse project file correctly: {0}", project.Path));
        }

        // If the project is an exe, then simply copy all of the contents to the correct output folder
        if(parsedProject.OutputType.ToLower() == "exe" || parsedProject.OutputType.ToLower() == "winexe") 
        {
            Information("Project has an output type of exe: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedApplications.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }

        var isWebProject = false;

        // Next up, we need to check if this is a web project.  Easiest way to check this is to see if there is a web.config file
        // If there is, simply copy the output files to the correct folder
        foreach(var file in parsedProject.Files)
        {
            Verbose("FilePath: {0}", file.FilePath.Path);
            if(file.FilePath.Path.ToLower().Contains("web.config"))
            {
                isWebProject = true;
                break;
            }
        }

        if(parsedProject.OutputType.ToLower() == "library" && isWebProject)
        {
            Information("Project has an output type of library and is a Web Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedApplications.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true); 
            continue;
        }

        var isxUnitTestProject = false;
        var ismsTestProject = false;

        // Now we need to test for whether this is a unit test project.  Currently, this is only testing for XUnit Projects.
        // It needs to be extended to include others, i.e. NUnit, MSTest, and VSTest
        // If this is found, move the output to the unit test folder, otherwise, simply copy to normal output folder
        foreach(var reference in parsedProject.References)
        {
            Verbose("Reference Include: {0}", reference.Include);
            if(reference.Include.ToLower().Contains("xunit.core"))
            {
                isxUnitTestProject = true;
                break;
            }
            else if(reference.Include.ToLower().Contains("unittestframework"))
            {
                ismsTestProject = true;
                break;
            }
        }

        if(parsedProject.OutputType.ToLower() == "library" && isxUnitTestProject)
        {
            Information("Project has an output type of library and is an xUnit Test Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedxUnitTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true); 
            continue;
        }
        else if(parsedProject.OutputType.ToLower() == "library" && ismsTestProject)
        {
            // We will use vstest.console.exe by default for MSTest Projects
            Information("Project has an output type of library and is an MSTest Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedVSTestTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true); 
            continue;
        }
        else
        {
            Information("Project has an output type of library: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedLibraries.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true); 
            continue;
        }
    }
}

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
  
Task("PublishDocs")
    .IsDependentOn("Force-Publish-Documentation");
    
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
