///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;
var currentSupportedCakeVersionNumber = "0.33.0.0";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup<BuildData>(context =>
{
    Information(Figlet(BuildParameters.Title));

    Information("Starting Setup...");

    if (BuildParameters.BranchType == BranchType.Master && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    RequireTool(ToolSettings.GitVersionTool, () => {
        BuildParameters.SetBuildVersion(
            BuildVersion.CalculatingSemanticVersion(
                context: Context
            )
        );
    });

    Information("Building version {0} of " + BuildParameters.Title + " ({1}, {2}) using version {3} of Cake, and version {4} of Cake.Recipe. (IsTagged: {5})",
        BuildParameters.Version.SemVersion,
        BuildParameters.Configuration,
        BuildParameters.Target,
        BuildParameters.Version.CakeVersion,
        BuildMetaData.Version,
        BuildParameters.IsTagged);

    if (BuildParameters.Version.CakeVersion != currentSupportedCakeVersionNumber)
    {
        throw new Exception(string.Format("Cake.Recipe currently only supports building projects using version {0} of Cake.  Please update your packages.config file (or whatever method is used to pin to a specific version of Cake) to use this version.", currentSupportedCakeVersionNumber));
    }

    // Make sure build and linters run before issues task.
    IssuesBuildTasks.ReadIssuesTask
        .IsDependentOn("Build")
        .IsDependentOn("InspectCode");

    return new BuildData(context);
});

Teardown(context =>
{
    Information("Starting Teardown...");

    if (context.Successful)
    {
        if (!BuildParameters.IsLocalBuild && !BuildParameters.IsPullRequest && BuildParameters.IsMainRepository && (BuildParameters.BranchType == BranchType.Master || ((BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix) && BuildParameters.ShouldNotifyBetaReleases)) && BuildParameters.IsTagged)
        {
            if (BuildParameters.CanPostToTwitter && BuildParameters.ShouldPostToTwitter)
            {
                SendMessageToTwitter();
            }

            if (BuildParameters.CanPostToGitter && BuildParameters.ShouldPostToGitter)
            {
                SendMessageToGitterRoom();
            }

            if (BuildParameters.CanPostToMicrosoftTeams && BuildParameters.ShouldPostToMicrosoftTeams)
            {
                SendMessageToMicrosoftTeams();
            }

            if (BuildParameters.CanSendEmail && BuildParameters.ShouldSendEmail && !string.IsNullOrEmpty(BuildParameters.EmailRecipient))
            {
                var subject = $"Continuous Integration Build of {BuildParameters.Title} completed successfully";
                var message = new StringBuilder();
                message.AppendLine(BuildParameters.StandardMessage + "<br/>");
                message.AppendLine("<br/>");
                message.AppendLine($"<strong>Name</strong>: {BuildParameters.Title}<br/>");
                message.AppendLine($"<strong>Version</strong>: {BuildParameters.Version.SemVersion}<br/>");
                message.AppendLine($"<strong>Configuration</strong>: {BuildParameters.Configuration}<br/>");
                message.AppendLine($"<strong>Target</strong>: {BuildParameters.Target}<br/>");
                message.AppendLine($"<strong>Cake version</strong>: {BuildParameters.Version.CakeVersion}<br/>");
                message.AppendLine($"<strong>Cake.Recipe version</strong>: {BuildMetaData.Version}<br/>");

                SendEmail(subject, message.ToString(), BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
            }
        }
    }
    else
    {
        if (!BuildParameters.IsLocalBuild && BuildParameters.IsMainRepository)
        {
            if (BuildParameters.CanPostToSlack && BuildParameters.ShouldPostToSlack)
            {
                SendMessageToSlackChannel("Continuous Integration Build of " + BuildParameters.Title + " just failed :-(");
            }

            if (BuildParameters.CanSendEmail && BuildParameters.ShouldSendEmail && !string.IsNullOrEmpty(BuildParameters.EmailRecipient))
            {
                var subject = $"Continuous Integration Build of {BuildParameters.Title} failed";
                var message = context.ThrownException.ToString().Replace(System.Environment.NewLine, "<br/>");

                SendEmail(subject, message, BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
            }
        }
    }

    // Clear nupkg files from tools directory
    if ((!BuildParameters.IsLocalBuild || BuildParameters.ShouldDeleteCachedFiles) && DirectoryExists(Context.Environment.WorkingDirectory.Combine("tools")))
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
    Information("PrepareLocalRelease: {0}", BuildParameters.PrepareLocalRelease);
    Information("ShouldDownloadMilestoneReleaseNotes: {0}", BuildParameters.ShouldDownloadMilestoneReleaseNotes);
    Information("ShouldDownloadFullReleaseNotes: {0}", BuildParameters.ShouldDownloadFullReleaseNotes);
    Information("IsLocalBuild: {0}", BuildParameters.IsLocalBuild);
    Information("IsPullRequest: {0}", BuildParameters.IsPullRequest);
    Information("IsMainRepository: {0}", BuildParameters.IsMainRepository);
    Information("IsTagged: {0}", BuildParameters.IsTagged);

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

    NuGetRestore(
        BuildParameters.SolutionFilePath,
        new NuGetRestoreSettings
        {
            Source = BuildParameters.NuGetSources
        });
});

BuildParameters.Tasks.DotNetCoreRestoreTask = Task("DotNetCore-Restore")
    .Does(() =>
{
    var msBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", BuildParameters.Version.SemVersion)
                            .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
                            .WithProperty("FileVersion",  BuildParameters.Version.Version)
                            .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

    if (BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
    {
        var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        // Use FrameworkPathOverride when not running on Windows.
        Information("Restore will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
        msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }

    DotNetCoreRestore(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreRestoreSettings
    {
        Sources = BuildParameters.NuGetSources,
        MSBuildSettings = msBuildSettings,
        PackagesDirectory = BuildParameters.RestorePackagesDirectory
    });
});

BuildParameters.Tasks.BuildTask = Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does<BuildData>(data => RequireTool(ToolSettings.MSBuildExtensionPackTool, () => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows)
        {
            var msbuildSettings = new MSBuildSettings()
                .SetPlatformTarget(ToolSettings.BuildPlatformTarget)
                .UseToolVersion(ToolSettings.BuildMSBuildToolVersion)
                .WithProperty("TreatWarningsAsErrors", BuildParameters.TreatWarningsAsErrors.ToString())
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

            MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);

            // Pass path to MsBuild log file to Cake.Issues.Recipe
            IssuesParameters.InputFiles.MsBuildXmlFileLoggerLogFilePath = BuildParameters.Paths.Files.BuildLogFilePath;
        }
        else
        {
            var xbuildSettings = new XBuildSettings()
                .SetConfiguration(BuildParameters.Configuration)
                .WithTarget("Build")
                .WithProperty("TreatWarningsAsErrors", "true");

            XBuild(BuildParameters.SolutionFilePath, xbuildSettings);
        }

        CopyBuildOutput();
    }));


BuildParameters.Tasks.DotNetCoreBuildTask = Task("DotNetCore-Build")
    .IsDependentOn("Clean")
    .IsDependentOn("DotNetCore-Restore")
    .Does(() => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        var msBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", BuildParameters.Version.SemVersion)
                            .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
                            .WithProperty("FileVersion",  BuildParameters.Version.Version)
                            .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

        if (BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
        {
            var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

            // Use FrameworkPathOverride when not running on Windows.
            Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
            msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
        }

        DotNetCoreBuild(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = BuildParameters.Configuration,
            MSBuildSettings = msBuildSettings,
            NoRestore = true
        });

        CopyBuildOutput();
    });

public void CopyBuildOutput()
{
    Information("Copying build output...");

    foreach (var project in ParseSolution(BuildParameters.SolutionFilePath).GetProjects())
    {
        // There is quite a bit of duplication in this function, that really needs to be tidied Upload

        Information("Input BuildPlatformTarget: {0}", ToolSettings.BuildPlatformTarget.ToString());
        var platformTarget = ToolSettings.BuildPlatformTarget == PlatformTarget.MSIL ? "AnyCPU" : ToolSettings.BuildPlatformTarget.ToString();
        Information("Using BuildPlatformTarget: {0}", platformTarget);
        var parsedProject = ParseProject(project.Path, BuildParameters.Configuration, platformTarget);

        if (project.Path.FullPath.ToLower().Contains("wixproj"))
        {
            Warning("Skipping wix project");
            continue;
        }

        if (project.Path.FullPath.ToLower().Contains("shproj"))
        {
            Warning("Skipping shared project");
            continue;
        }

        if (parsedProject.OutputPath == null || parsedProject.RootNameSpace == null || parsedProject.OutputType == null)
        {
            Information("OutputPath: {0}", parsedProject.OutputPath);
            Information("RootNameSpace: {0}", parsedProject.RootNameSpace);
            Information("OutputType: {0}", parsedProject.OutputType);
            throw new Exception(string.Format("Unable to parse project file correctly: {0}", project.Path));
        }

        // If the project is an exe, then simply copy all of the contents to the correct output folder
        if (!parsedProject.IsLibrary())
        {
            Information("Project has an output type of exe: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedApplications.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);

            // If .NET SDK project, copy using dotnet publish for each target framework
            // Otherwise just copy
            if (parsedProject.IsVS2017ProjectFormat)
            {
                var msBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", BuildParameters.Version.SemVersion)
                            .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
                            .WithProperty("FileVersion",  BuildParameters.Version.Version)
                            .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

                if (BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
                {
                    var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

                    // Use FrameworkPathOverride when not running on Windows.
                    Information("Publish will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
                    msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
                }

                foreach (var targetFramework in parsedProject.NetCore.TargetFrameworks)
                {
                    DotNetCorePublish(project.Path.FullPath, new DotNetCorePublishSettings {
                        OutputDirectory = outputFolder.Combine(targetFramework),
                        Framework = targetFramework,
                        Configuration = BuildParameters.Configuration,
                        MSBuildSettings = msBuildSettings,
                        NoRestore = true,
                        NoBuild = true
                    });
                }
            }
            else
            {
                CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            }

            continue;
        }

        // Now we need to test for whether this is a unit test project.
        // If this is found, move the output to the unit test folder, otherwise, simply copy to normal output folder
        if (!BuildParameters.IsDotNetCoreBuild)
        {
            Information("Not a .Net Core Build");
        }
        else
        {
            Information("Is a .Net Core Build");
        }

        if (parsedProject.IsLibrary() && (parsedProject.HasPackage("xunit") || parsedProject.HasReference("xunit.core")))
        {
            Information("Project has an output type of library and is an xUnit Test Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedxUnitTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if (parsedProject.IsLibrary() && (parsedProject.HasPackage("fixie") || parsedProject.HasReference("fixie")))
        {
            Information("Project has an output type of library and is a Fixie Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedFixieTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if (parsedProject.IsLibrary() && (parsedProject.HasPackage("nunit") || parsedProject.HasReference("nunit.framework")))
        {
            Information("Project has an output type of library and is a NUnit Test Project: {0}", parsedProject.RootNameSpace);
            var outputFolder = BuildParameters.Paths.Directories.PublishedNUnitTests.Combine(parsedProject.RootNameSpace);
            EnsureDirectoryExists(outputFolder);
            CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            continue;
        }
        else if (parsedProject.IsLibrary() && parsedProject.IsMSTestProject())
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

            // If .NET SDK project, copy for each output path
            // Otherwise just copy
            if (parsedProject.IsVS2017ProjectFormat)
            {
                foreach (var outputPath in parsedProject.OutputPaths)
                {
                    var outputFolder = BuildParameters.Paths.Directories.PublishedLibraries.Combine(parsedProject.RootNameSpace).Combine(outputPath.GetDirectoryName());
                    EnsureDirectoryExists(outputFolder);
                    Information(outputPath);
                    CopyFiles(GetFiles(outputPath + "/**/*"), outputFolder, true);
                }
            }
            else
            {
                var outputFolder = BuildParameters.Paths.Directories.PublishedLibraries.Combine(parsedProject.RootNameSpace);
                EnsureDirectoryExists(outputFolder);
                Information(parsedProject.OutputPath.FullPath);
                CopyFiles(GetFiles(parsedProject.OutputPath.FullPath + "/**/*"), outputFolder, true);
            }
            continue;
        }
    }
}

BuildParameters.Tasks.PackageTask = Task("Package")
    .IsDependentOn("Export-Release-Notes");

BuildParameters.Tasks.DefaultTask = Task("Default")
    .IsDependentOn("Package")
    // Run issues task from Cake.Issues.Recipe by default.
    .IsDependentOn("Issues");

BuildParameters.Tasks.UploadArtifactsTask = Task("Upload-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages) || DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    // Concatenating FilePathCollections should make sure we get unique FilePaths
    foreach (var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
    {
        BuildParameters.BuildProvider.UploadArtifact(package);
    }
});

BuildParameters.Tasks.ContinuousIntegrationTask = Task("ContinuousIntegration")
    .IsDependentOn("Upload-Artifacts")
    .IsDependentOn("Publish-PreRelease-Packages")
    .IsDependentOn("Publish-Release-Packages")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-Documentation")
    .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + BuildParameters.Title + ".  All publishing tasks have been attempted.");
    }
});

BuildParameters.Tasks.UploadCoverageReportTask = Task("Upload-Coverage-Report")
  .IsDependentOn("Upload-Coveralls-Report")
  .IsDependentOn("Upload-Codecov-Report");

BuildParameters.Tasks.ReleaseNotesTask = Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

BuildParameters.Tasks.LabelsTask = Task("Labels")
  .IsDependentOn("Create-Default-Labels");

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
        BuildParameters.IsDotNetCoreBuild = false;
        BuildParameters.IsNuGetBuild = false;

        SetupTasks(BuildParameters.IsDotNetCoreBuild);

        _action(BuildParameters.Target);
    }

    public void RunDotNetCore()
    {
        BuildParameters.IsDotNetCoreBuild = true;
        BuildParameters.IsNuGetBuild = false;

        SetupTasks(BuildParameters.IsDotNetCoreBuild);

        _action(BuildParameters.Target);
    }

    public void RunNuGet()
    {
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Package");
        BuildParameters.IsDotNetCoreBuild = false;
        BuildParameters.IsNuGetBuild = true;

        _action(BuildParameters.Target);
    }

    private static void SetupTasks(bool isDotNetCoreBuild)
    {
        var prefix = isDotNetCoreBuild ? "DotNetCore-" : "";
        BuildParameters.Tasks.CreateNuGetPackagesTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.CreateChocolateyPackagesTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.TestTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.DupFinderTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.InspectCodeTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Analyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Packages");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Chocolatey-Packages");
        BuildParameters.Tasks.UploadCodecovReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.UploadCoverallsReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.ContinuousIntegrationTask.IsDependentOn("Upload-Coverage-Report");
        BuildParameters.Tasks.InstallReportGeneratorTask.IsDependentOn(prefix + "Build");

        if (!isDotNetCoreBuild)
        {
            if (BuildParameters.TransifexEnabled)
            {
                BuildParameters.Tasks.BuildTask.IsDependentOn("Transifex-Pull-Translations");
            }
            BuildParameters.Tasks.TestTask.IsDependentOn("Test-NUnit");
            BuildParameters.Tasks.TestTask.IsDependentOn("Test-xUnit");
            BuildParameters.Tasks.TestTask.IsDependentOn("Test-MSTest");
            BuildParameters.Tasks.TestTask.IsDependentOn("Test-VSTest");
            BuildParameters.Tasks.TestTask.IsDependentOn("Test-Fixie");
            BuildParameters.Tasks.InstallOpenCoverTask.IsDependentOn("Install-ReportUnit");
        }
        else
        {
            if (BuildParameters.TransifexEnabled)
            {
                BuildParameters.Tasks.DotNetCoreBuildTask.IsDependentOn("Transifex-Pull-Translations");
            }
            BuildParameters.Tasks.TestTask.IsDependentOn(prefix + "Test");
            BuildParameters.Tasks.InstallOpenCoverTask.IsDependentOn("Install-ReportGenerator");
            BuildParameters.Tasks.PackageTask.IsDependentOn(prefix + "Pack");
        }
    }
}
