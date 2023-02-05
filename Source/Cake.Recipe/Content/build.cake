///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// Support function for comparing cake version support
///////////////////////////////////////////////////////////////////////////////
public bool IsSupportedCakeVersion(string supportedVersion, string currentVersion)
{
    var onePartSupported = Version.Parse(supportedVersion).ToString(1);
    var onePartCurrent = Version.Parse(currentVersion).ToString(1);

    return onePartCurrent == onePartSupported;
}

///////////////////////////////////////////////////////////////////////////////
// TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Teardown<BuildVersion>((context, buildVersion) =>
{
    Information("Starting Teardown...");

    if (BuildParameters.PublishReleasePackagesWasSuccessful)
    {
        if (!BuildParameters.IsLocalBuild &&
            !BuildParameters.IsPullRequest &&
            BuildParameters.IsMainRepository &&
            (BuildParameters.BranchType == BranchType.Master ||
                ((BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix) &&
                BuildParameters.ShouldNotifyBetaReleases)) &&
            BuildParameters.IsTagged &&
            !BuildParameters.IsRunningIntegrationTests)
        {
            var messageArguments = BuildParameters.MessageArguments(buildVersion);
            foreach(var reporter in BuildParameters.SuccessReporters)
            {
                if(reporter.ShouldBeUsed && reporter.CanBeUsed)
                {
                    reporter.ReportSuccess(context, buildVersion);
                }
            }
        }
    }

    if(!context.Successful)
    {
        if (!BuildParameters.IsLocalBuild &&
            BuildParameters.IsMainRepository &&
            !BuildParameters.IsRunningIntegrationTests)
        {
            foreach(var reporter in BuildParameters.FailureReporters)
            {
                if(reporter.ShouldBeUsed && reporter.CanBeUsed)
                {
                    reporter.ReportFailure(context, buildVersion, context.ThrownException);
                }
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
    Information("Build Platform: {0}", BuildParameters.Platform);
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
    Information("Source DirectoryPath: {0}", MakeAbsolute(BuildParameters.SourceDirectoryPath));
    Information("Build DirectoryPath: {0}", MakeAbsolute(BuildParameters.Paths.Directories.Build));
});

BuildParameters.Tasks.CleanTask = Task("Clean")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-CI-Provider-Environment-Variables")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
});

BuildParameters.Tasks.RestoreTask = Task("Restore")
    .Does(() =>
{
    Information("Restoring {0}...", BuildParameters.SolutionFilePath);
    RequireToolNotRegistered(ToolSettings.NuGetTool, new[] { "nuget.exe" }, () => {
        NuGetRestore(
            BuildParameters.SolutionFilePath,
            new NuGetRestoreSettings
            {
                Source = BuildParameters.NuGetSources
            });
    });
});

BuildParameters.Tasks.DotNetCoreRestoreTask = Task("DotNetCore-Restore")
    .Does<BuildVersion>((context, buildVersion) =>
{
    // We need to clone the settings class, so we don't
    // add additional properties to every other task.
    var msBuildSettings = new DotNetCoreMSBuildSettings();
    foreach (var kv in context.Data.Get<DotNetCoreMSBuildSettings>().Properties)
    {
        string value = string.Join(" ", kv.Value);
        msBuildSettings.WithProperty(kv.Key, value);
    }
    msBuildSettings.WithProperty("Configuration", BuildParameters.Configuration);

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
    .Does<BuildVersion>((context, buildVersion) => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows || Context.Tools.Resolve("msbuild") != null)
        {
            var msbuildSettings = new MSBuildSettings()
                .SetPlatformTarget(ToolSettings.BuildPlatformTarget)
                .UseToolVersion(ToolSettings.BuildMSBuildToolVersion)
                .WithProperty("TreatWarningsAsErrors", BuildParameters.TreatWarningsAsErrors.ToString())
                .WithTarget("Build")
                .SetMaxCpuCount(ToolSettings.MaxCpuCount)
                .SetConfiguration(BuildParameters.Configuration);

            msbuildSettings.ArgumentCustomization = args =>
                args.Append(string.Format("/logger:BinaryLogger,\"{0}\";\"{1}\"",
                    context.Tools.Resolve("Cake.Issues.MsBuild*/**/StructuredLogger.dll"),
                    BuildParameters.Paths.Files.BuildBinLogFilePath));

            // This is used in combination with SourceLink to ensure a deterministic
            // package is generated
            if(BuildParameters.ShouldUseDeterministicBuilds)
            {
                msbuildSettings.WithProperty("ContinuousIntegrationBuild", "true");
            }

            MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);

            // Pass path to MsBuild log file to Cake.Issues.Recipe
            IssuesParameters.InputFiles.AddMsBuildBinaryLogFile(BuildParameters.Paths.Files.BuildBinLogFilePath);
        }
        else
        {
            var xbuildSettings = new XBuildSettings()
                .SetConfiguration(BuildParameters.Configuration)
                .WithTarget("Build")
                .WithProperty("TreatWarningsAsErrors", "true");

            XBuild(BuildParameters.SolutionFilePath, xbuildSettings);
        }

        CopyBuildOutput(buildVersion);
    });


BuildParameters.Tasks.DotNetCoreBuildTask = Task("DotNetCore-Build")
    .IsDependentOn("Clean")
    .IsDependentOn("DotNetCore-Restore")
    .Does<BuildVersion>((context, buildVersion) => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        // We need to clone the settings class, so we don't
        // add additional properties to every other task.
        var msBuildSettings = new DotNetCoreMSBuildSettings();
        foreach (var kv in context.Data.Get<DotNetCoreMSBuildSettings>().Properties)
        {
            string value = string.Join(" ", kv.Value);
            msBuildSettings.WithProperty(kv.Key, value);
        }
        msBuildSettings.WithLogger("BinaryLogger," + context.Tools.Resolve("Cake.Issues.MsBuild*/**/StructuredLogger.dll"),
            "",
            BuildParameters.Paths.Files.BuildBinLogFilePath.ToString());

        DotNetCoreBuild(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = BuildParameters.Configuration,
            MSBuildSettings = msBuildSettings,
            NoRestore = true
        });

        // We set this here, so we won't have a failure in case this task is never called
        IssuesParameters.InputFiles.AddMsBuildBinaryLogFile(BuildParameters.Paths.Files.BuildBinLogFilePath);

        CopyBuildOutput(buildVersion);
    });

public void CopyBuildOutput(BuildVersion buildVersion)
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

        if (parsedProject.OutputPaths.FirstOrDefault() == null || parsedProject.RootNameSpace == null || parsedProject.OutputType == null)
        {
            Information("OutputPath: {0}", parsedProject.OutputPaths.FirstOrDefault());
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
                var msBuildSettings = Context.Data.Get<DotNetCoreMSBuildSettings>();

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
                CopyMsBuildProjectOutput(outputFolder, parsedProject);
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
            CopyMsBuildProjectOutput(BuildParameters.Paths.Directories.PublishedxUnitTests, parsedProject);
        }
        else if (parsedProject.IsLibrary() && (parsedProject.HasPackage("nunit") || parsedProject.HasReference("nunit.framework")))
        {
            Information("Project has an output type of library and is a NUnit Test Project: {0}", parsedProject.RootNameSpace);
            CopyMsBuildProjectOutput(BuildParameters.Paths.Directories.PublishedNUnitTests, parsedProject);
        }
        else if (parsedProject.IsLibrary() && parsedProject.IsMSTestProject())
        {
            // We will use vstest.console.exe by default for MSTest Projects
            Information("Project has an output type of library and is an MSTest Project: {0}", parsedProject.RootNameSpace);
            CopyMsBuildProjectOutput(BuildParameters.Paths.Directories.PublishedVSTestTests, parsedProject);
        }
        else
        {
            Information("Project has an output type of library: {0}", parsedProject.RootNameSpace);
            CopyMsBuildProjectOutput(BuildParameters.Paths.Directories.PublishedLibraries, parsedProject);
        }
    }
}

public void CopyMsBuildProjectOutput(DirectoryPath outputBase, CustomProjectParserResult parsedProject)
{
    if (parsedProject.IsVS2017ProjectFormat)
    {
        foreach (var outputPath in parsedProject.OutputPaths)
        {
            var outputFolder = outputBase.Combine(parsedProject.RootNameSpace).Combine(outputPath.GetDirectoryName());
            EnsureDirectoryExists(outputFolder);
            var files = GetFiles(outputPath + "/**/*");
            if (files.Any())
            {
                CopyFiles(files, outputFolder, true);
            }
            else
            {
                Warning("No files were found in the project output directory: '{0}'", outputPath);
            }
        }
    }
    else
    {
        var outputFolder = outputBase.Combine(parsedProject.RootNameSpace);
        EnsureDirectoryExists(outputFolder);
        var outputPath = parsedProject.OutputPaths.First().FullPath;
        var files = GetFiles(outputPath + "/**/*");
        if (files.Any())
        {
            CopyFiles(files, outputFolder, true);
        }
        else
        {
            Warning("No files were found in the project output directory: '{0}'", outputPath);
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
    var artifacts = GetFiles(BuildParameters.Paths.Directories.Packages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.TestCoverage + "/coverlet/*.xml");

    if (FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
    {
        artifacts += BuildParameters.Paths.Files.TestCoverageOutputFilePath;
    }

    foreach (var artifact in artifacts)
    {
        BuildParameters.BuildProvider.UploadArtifact(artifact);
    }
});

BuildParameters.Tasks.ContinuousIntegrationTask = Task("CI")
    // Run issues task from Cake.Issues.Recipe by default.
    .IsDependentOn("Upload-Artifacts")
    .IsDependentOn("Issues")
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
        BuildParameters.Tasks.InspectCodeTask.IsDependentOn(prefix + "Build");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Analyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-NuGet-Packages");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Chocolatey-Packages");
        BuildParameters.Tasks.UploadCodecovReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.UploadCoverallsReportTask.IsDependentOn("Test");
        BuildParameters.Tasks.ContinuousIntegrationTask.IsDependentOn("Upload-Coverage-Report");

        if (!isDotNetCoreBuild)
        {
            if (BuildParameters.TransifexEnabled)
            {
                BuildParameters.Tasks.BuildTask.IsDependentOn("Transifex-Pull-Translations");
            }

            BuildParameters.Tasks.TestMSTestTask.IsDependentOn(prefix + "Build");
            BuildParameters.Tasks.TestNUnitTask.IsDependentOn(prefix + "Build");
            BuildParameters.Tasks.TestVSTestTask.IsDependentOn(prefix + "Build");
            BuildParameters.Tasks.TestxUnitTask.IsDependentOn(prefix + "Build");
            BuildParameters.Tasks.GenerateLocalCoverageReportTask.IsDependentOn("Test-MSTest");
            BuildParameters.Tasks.GenerateLocalCoverageReportTask.IsDependentOn("Test-NUnit");
            BuildParameters.Tasks.GenerateLocalCoverageReportTask.IsDependentOn("Test-VSTest");
            BuildParameters.Tasks.GenerateLocalCoverageReportTask.IsDependentOn("Test-xUnit");
            BuildParameters.Tasks.TestTask.IsDependentOn("Generate-FriendlyTestReport");
            BuildParameters.Tasks.TestTask.IsDependentOn("Generate-LocalCoverageReport");
        }
        else
        {
            if (BuildParameters.TransifexEnabled)
            {
                BuildParameters.Tasks.DotNetCoreBuildTask.IsDependentOn("Transifex-Pull-Translations");
            }
            BuildParameters.Tasks.GenerateLocalCoverageReportTask.IsDependentOn(prefix + "Test");
            BuildParameters.Tasks.TestTask.IsDependentOn("Generate-LocalCoverageReport");
            BuildParameters.Tasks.PackageTask.IsDependentOn(prefix + "Pack");
        }
    }
}
