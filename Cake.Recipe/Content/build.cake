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

    RequireTool(GitVersionTool, () => {
        BuildParameters.SetBuildVersion(
            BuildVersion.CalculatingSemanticVersion(
                context: Context
            )
        );
    });

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
        if(!BuildParameters.IsLocalBuild && !BuildParameters.IsPullRequest && BuildParameters.IsMainRepository && (BuildParameters.IsMasterBranch || ((BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch) && BuildParameters.ShouldNotifyBetaReleases)) && BuildParameters.IsTagged)
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

    // Clear nupkg files from tools directory
    if(DirectoryExists(Context.Environment.WorkingDirectory.Combine("tools")))
    {
        Information("Deleting nupkg files...");
        var nupkgFiles = GetFiles(Context.Environment.WorkingDirectory.Combine("tools") + "/**/*.nupkg");
        DeleteFiles(nupkgFiles);
    }

    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.ShowInfoTask = Task("Show-Info")
    .Does(() =>
{
    Information("Target: {0}", BuildParameters.Target);
    Information("Configuration: {0}", BuildParameters.Configuration);

    Information("Solution FilePath: {0}", MakeAbsolute((FilePath)BuildParameters.SolutionFilePath));
    Information("Solution DirectoryPath: {0}", MakeAbsolute((DirectoryPath)BuildParameters.SolutionDirectoryPath));
    Information("Source DirectoryPath: {0}", MakeAbsolute(BuildParameters.SourceDirectoryPath));
    Information("Build DirectoryPath: {0}", MakeAbsolute(BuildParameters.Paths.Directories.Build));
});

BuildParameters.Tasks.CleanTask = Task("Clean")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-AppVeyor-Environment-Variables")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
});

BuildParameters.Tasks.RestoreTask = Task("Restore")
    .Does(() =>
{
    Information("Restoring {0}...", BuildParameters.SolutionFilePath);

    // TODO Use parameter for Cake Contrib feed from environment variable, similar to BuildParameters.MyGet.SourceUrl
    NuGetRestore(
        BuildParameters.SolutionFilePath,
        new NuGetRestoreSettings
        {
            Source = new List<string>
            {
                "https://www.nuget.org/api/v2",
                "https://www.myget.org/F/cake-contrib/api/v2"
            }
        });
});

BuildParameters.Tasks.DotNetCoreRestoreTask = Task("DotNetCore-Restore")
    .Does(() =>
{
    DotNetCoreRestore(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreRestoreSettings
    {
        Verbose = false,
        Sources = new [] {
            "https://api.nuget.org/v3/index.json",
            "https://www.myget.org/F/cake-contrib/api/v3/index.json"
        }
    });
});

BuildParameters.Tasks.BuildTask = Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => RequireTool(MSBuildExtensionPackTool, () => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        var msbuildSettings = new MSBuildSettings().
                SetPlatformTarget(ToolSettings.BuildPlatformTarget)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithTarget("Build")
                .SetMaxCpuCount(ToolSettings.MaxCpuCount)
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
    }))
    .Finally(() => {
        CreateCodeAnalysisReport();
    });


BuildParameters.Tasks.DotNetCoreBuildTask = Task("DotNetCore-Build")
    .IsDependentOn("Clean")
    .IsDependentOn("DotNetCore-Restore")
    .Does(() => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        DotNetCoreBuild(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = BuildParameters.Configuration,
            ArgumentCustomization = args => args
                .Append("/p:Version={0}", BuildParameters.Version.SemVersion)
                .Append("/p:AssemblyVersion={0}", BuildParameters.Version.Version)
                .Append("/p:FileVersion={0}", BuildParameters.Version.Version)
                .Append("/p:AssemblyInformationalVersion={0}", BuildParameters.Version.InformationalVersion)
        });

        if(BuildParameters.ShouldExecuteGitLink)
        {
            ExecuteGitLink();
        }

        CopyBuildOutput();
    });

public void CreateCodeAnalysisReport()
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.CodeAnalysisResults);

    Information("Create MsBuild code analysis report by rule...");
    var fileName = BuildParameters.Paths.Directories.CodeAnalysisResults.CombineWithFilePath("ByRule.html");
    CreateMsBuildCodeAnalysisReport(
        BuildParameters.Paths.Files.BuildLogFilePath,
        CodeAnalysisReport.MsBuildXmlFileLoggerByRule,
        fileName);
    Information("MsBuild code analysis report by rule was written to: {0}", fileName.FullPath);

    Information("Create MsBuild code analysis report by assembly...");
    fileName = BuildParameters.Paths.Directories.CodeAnalysisResults.CombineWithFilePath("ByAssembly.html");
    CreateMsBuildCodeAnalysisReport(
        BuildParameters.Paths.Files.BuildLogFilePath,
        CodeAnalysisReport.MsBuildXmlFileLoggerByAssembly,
        BuildParameters.Paths.Directories.CodeAnalysisResults.CombineWithFilePath("ByAssembly.html"));
    Information("MsBuild code analysis report by assembly was written to: {0}", fileName.FullPath);
}

public void CopyBuildOutput()
{
    Information("Copying build output...");

    foreach(var project in ParseSolution(BuildParameters.SolutionFilePath).GetProjects())
    {
        // There is quite a bit of duplication in this function, that really needs to be tidied Upload

        var parsedProject = ParseProject(project.Path, BuildParameters.Configuration);

        if(project.Path.FullPath.ToLower().Contains("wixproj"))
        {
            Warning("Skipping wix project");
            continue;
        }

        if(project.Path.FullPath.ToLower().Contains("shproj"))
        {
            Warning("Skipping shared project");
            continue;
        }

        if(parsedProject.OutputPath == null || parsedProject.RootNameSpace == null || parsedProject.OutputType == null)
        {
            throw new Exception(string.Format("Unable to parse project file correctly: {0}", project.Path));
        }

        // If the project is an exe, then simply copy all of the contents to the correct output folder
        if(!parsedProject.IsLibrary())
        {
            Information("Project has an output type of exe: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedApplications.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }

        var isxUnitTestProject = false;
        var ismsTestProject = false;
        var isFixieProject = false;
        var isNUnitProject = false;

        // Now we need to test for whether this is a unit test project.  Currently, this is only testing for XUnit Projects.
        // It needs to be extended to include others, i.e. NUnit, MSTest, and VSTest
        // If this is found, move the output to the unit test folder, otherwise, simply copy to normal output folder

        ICollection<ProjectAssemblyReference> references = null;
        if(!BuildParameters.IsDotNetCoreBuild)
        {
            Information("Not a .Net Core Build");
            references = parsedProject.References;
        }
        else
        {
            Information("Is a .Net Core Build");
            references = new List<ProjectAssemblyReference>();
        }

        foreach(var reference in references)
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
            else if(reference.Include.ToLower().Contains("fixie"))
            {
                isFixieProject = true;
                break;
            }
            else if(reference.Include.ToLower().Contains("nunit.framework"))
            {
                isNUnitProject = true;;
                break;
            }
        }

        if(parsedProject.IsLibrary() && isxUnitTestProject)
        {
            Information("Project has an output type of library and is an xUnit Test Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedxUnitTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if(parsedProject.IsLibrary() && ismsTestProject)
        {
            // We will use vstest.console.exe by default for MSTest Projects
            Information("Project has an output type of library and is an MSTest Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedVSTestTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if(parsedProject.IsLibrary() && isFixieProject)
        {
            Information("Project has an output type of library and is a Fixie Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedFixieTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if(parsedProject.IsLibrary() && isNUnitProject)
        {
            Information("Project has an output type of library and is a NUnit Test Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedNUnitTests.Combine(parsedProject.RootNameSpace);
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

BuildParameters.Tasks.PackageTask = Task("Package")
    .IsDependentOn("Export-Release-Notes");

BuildParameters.Tasks.DefaultTask = Task("Default")
    .IsDependentOn("Package");

BuildParameters.Tasks.AppVeyorTask = Task("AppVeyor")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Publish-MyGet-Packages")
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

BuildParameters.Tasks.ReleaseNotesTask = Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

BuildParameters.Tasks.ClearCacheTask = Task("ClearCache")
  .IsDependentOn("Clear-AppVeyor-Cache");

BuildParameters.Tasks.PreviewTask = Task("Preview")
  .IsDependentOn("Preview-Documentation");

BuildParameters.Tasks.PublishDocsTask = Task("PublishDocs")
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
        BuildParameters.Tasks.CreateNuGetPackagesTask.IsDependentOn("Build");
        BuildParameters.Tasks.CreateChocolateyPackagesTask.IsDependentOn("Build");
        BuildParameters.Tasks.TestTask.IsDependentOn("Build");
        BuildParameters.Tasks.DupFinderTask.IsDependentOn("Build");
        BuildParameters.Tasks.InspectCodeTask.IsDependentOn("Build");
        BuildParameters.Tasks.TestTask.IsDependentOn("Test-NUnit");
        BuildParameters.Tasks.TestTask.IsDependentOn("Test-xUnit");
        BuildParameters.Tasks.TestTask.IsDependentOn("Test-MSTest");
        BuildParameters.Tasks.TestTask.IsDependentOn("Test-VSTest");
        BuildParameters.Tasks.TestTask.IsDependentOn("Test-Fixie");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Analyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Packages");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Chocolatey-Packages");
        BuildParameters.Tasks.UploadCodecovReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.UploadCoverageReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Upload-Codecov-Report");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Upload-Coverage-Report");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Publish-Chocolatey-Packages");
        BuildParameters.Tasks.InstallReportGeneratorTask.IsDependentOn("Build");
        BuildParameters.Tasks.InstallOpenCoverTask.IsDependentOn("Install-ReportUnit");
        BuildParameters.IsDotNetCoreBuild = false;
        BuildParameters.IsNuGetBuild = false;

        _action(BuildParameters.Target);
    }

    public void RunDotNetCore()
    {
        BuildParameters.Tasks.CreateNuGetPackagesTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.CreateChocolateyPackagesTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.TestTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.DupFinderTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.InspectCodeTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.TestTask.IsDependentOn("DotNetCore-Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Analyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Packages");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Chocolatey-Packages");
        BuildParameters.Tasks.UploadCodecovReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.UploadCoverageReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Upload-Codecov-Report");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Upload-Coverage-Report");
        BuildParameters.Tasks.AppVeyorTask.IsDependentOn("Publish-Chocolatey-Packages");
        BuildParameters.Tasks.InstallReportGeneratorTask.IsDependentOn("DotNetCore-Build");
        BuildParameters.Tasks.InstallOpenCoverTask.IsDependentOn("Install-ReportGenerator");
        BuildParameters.IsDotNetCoreBuild = true;
        BuildParameters.IsNuGetBuild = false;

        _action(BuildParameters.Target);
    }

    public void RunNuGet()
    {
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Package");
        BuildParameters.IsDotNetCoreBuild = false;
        BuildParameters.IsNuGetBuild = true;

        _action(BuildParameters.Target);
    }
}
