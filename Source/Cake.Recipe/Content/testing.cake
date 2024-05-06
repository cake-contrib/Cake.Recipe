///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestNUnitTask = Task("Test-NUnit")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedNUnitTests), "No published NUnit tests")
    .Does(() => RequireTool(ToolSettings.NUnitTool, () =>
    {
        var files = GetFiles(BuildParameters.Paths.Directories.PublishedNUnitTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll"));
        var settings = new NUnit3Settings
        {
            NoResults = true
        };

        if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows && (ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.OpenCover))
        {
            RunOpenCover(Context, tool => tool.NUnit3(files, settings));
        }
        else if (ToolSettings.CoverageTool == CoverageToolType.CoverletConsole || ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.CoverletAuto)
        {
            RunCoverletConsole(Context, (tool, file) => tool.NUnit3(file.FullPath, settings), files);
        }
        else
        {
            NUnit3(files, settings);
        }
    })
);

BuildParameters.Tasks.TestxUnitTask = Task("Test-xUnit")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedxUnitTests), "No published xUnit tests")
    .Does(() => RequireTool(ToolSettings.XUnitTool, () => {
    EnsureDirectoryExists(BuildParameters.Paths.Directories.xUnitTestResults);

    var files = GetFiles(BuildParameters.Paths.Directories.PublishedxUnitTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll"));
    var settings = new XUnit2Settings
    {
        OutputDirectory = BuildParameters.Paths.Directories.xUnitTestResults,
        XmlReport = true,
        NoAppDomain = true
    };

    if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows && (ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.OpenCover))
    {
        RunOpenCover(Context, tool => tool.XUnit2(files, settings));
    }
    else if (ToolSettings.CoverageTool == CoverageToolType.CoverletConsole || ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.CoverletAuto)
    {
        RunCoverletConsole(Context, (tool, file) => tool.XUnit2(file.FullPath, settings), files);
    }
    else
    {
        XUnit2(files, settings);
    }
}));

BuildParameters.Tasks.TestMSTestTask = Task("Test-MSTest")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedMSTestTests), "No published MSTest tests")
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.MSTestTestResults);
    var files = GetFiles(BuildParameters.Paths.Directories.PublishedMSTestTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll"));
    var settings = new MSTestSettings
    {
        NoIsolation = false
    };

    if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows && (ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.OpenCover))
    {
        RunOpenCover(Context, tool => tool.MSTest(files, settings));
    }
    else if (ToolSettings.CoverageTool == CoverageToolType.CoverletConsole || ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.CoverletAuto)
    {
        RunCoverletConsole(Context, (tool, file) => tool.MSTest(file.FullPath, settings), files);
    }
    else
    {
        MSTest(files, settings);
    }
});

BuildParameters.Tasks.TestVSTestTask = Task("Test-VSTest")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedVSTestTests), "No published VSTest tests")
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.VSTestTestResults);

    var files = GetFiles(BuildParameters.Paths.Directories.PublishedMSTestTests + (BuildParameters.TestFilePattern ?? "/**/*Tests.dll"));
    var settings = new VSTestSettings()
    {
        InIsolation = true
    };

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        settings.WithAppVeyorLogger();
    }

    if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows && (ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.OpenCover))
    {
        RunOpenCover(Context, tool => tool.VSTest(files, settings));
    }
    else if (ToolSettings.CoverageTool == CoverageToolType.CoverletConsole || ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.CoverletAuto)
    {
        RunCoverletConsole(Context, (tool, file) => tool.VSTest(file.FullPath, settings), files);
    }
    else
    {
        VSTest(files, settings);
    }
});

BuildParameters.Tasks.DotNetCoreTestTask = Task("DotNetCore-Test")
    .Does<DotNetCoreMSBuildSettings>((context, msBuildSettings) => {

    CleanDirectory(BuildParameters.Paths.Directories.TestCoverage.Combine("coverlet"));

    if (ToolSettings.CoverageTool == CoverageToolType.None)
    {
        var dotNetSettings = new DotNetCoreTestSettings
        {
            Configuration = BuildParameters.Configuration,
            NoBuild       = true,
        };

        DotNetCoreTest(BuildParameters.SolutionFilePath.FullPath, dotNetSettings);
        return;
    }

    var notRunProjects = new List<string>();

    if (ToolSettings.CoverageTool != CoverageToolType.OpenCover)
    {
        notRunProjects = RunCoverlet(context, msBuildSettings).ToList();
    }

    if ((notRunProjects.Count == 1 && string.IsNullOrEmpty(notRunProjects[0])) || ToolSettings.CoverageTool == CoverageToolType.OpenCover)
    {
        notRunProjects = context.GetFiles(BuildParameters.TestDirectoryPath + (BuildParameters.TestFilePattern ?? "/**/*Tests.csproj")).Select(f => f.FullPath).ToList();
    }

    var settings = new DotNetCoreTestSettings
    {
        Configuration = BuildParameters.Configuration,
        NoBuild = true,
    };

    if (ToolSettings.CoverageTool == CoverageToolType.CoverletAuto || ToolSettings.CoverageTool == CoverageToolType.CoverletConsole)
    {
        RunCoverletConsole(Context, (tool, file) => tool.DotNetCoreTest(file.FullPath, settings), notRunProjects);
    }
    else
    {
        foreach (var project in notRunProjects)
        {
            Action<ICakeContext> testAction = tool =>
            {
                tool.DotNetCoreTest(project, settings);
            };

            if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows)
            {
                RunOpenCover(context, testAction, registerUser: true);
            }
            else
            {
                testAction(context);
            }
        }
    }
});

BuildParameters.Tasks.IntegrationTestTask = Task("Run-Integration-Tests")
    .WithCriteria(() => BuildParameters.ShouldRunIntegrationTests, "Cake script integration tests have been disabled")
    .IsDependentOn("Default")
    .Does(() =>
    {
            CakeExecuteScript(BuildParameters.IntegrationTestScriptPath,
                new CakeSettings
                {
                    Arguments = new Dictionary<string, string>
                    {
                        { "verbosity", Context.Log.Verbosity.ToString("F") }
                    }
                });
    });

BuildParameters.Tasks.GenerateFriendlyTestReportTask = Task("Generate-FriendlyTestReport")
    .IsDependentOn("Test-VSTest")
    .IsDependentOn("Test-xUnit")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows")
    .Does(() => RequireTool(ToolSettings.ReportUnitTool, () =>
    {
        var possibleDirectories = new[] {
            BuildParameters.Paths.Directories.xUnitTestResults,
            BuildParameters.Paths.Directories.VSTestTestResults,
        };

        foreach (var directory in possibleDirectories.Where((d) => DirectoryExists(d)))
        {
            ReportUnit(directory, directory, new ReportUnitSettings());
        }
    })
);

BuildParameters.Tasks.GenerateLocalCoverageReportTask = Task("Generate-LocalCoverageReport")
    .WithCriteria(() => BuildParameters.IsLocalBuild, "Skipping due to not running a local build")
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.ReportGeneratorGlobalTool : ToolSettings.ReportGeneratorTool, () => {
        var coverageFiles = GetFiles(BuildParameters.Paths.Directories.TestCoverage + "/coverlet/**/*.*");
        if (FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
        {
            coverageFiles += BuildParameters.Paths.Files.TestCoverageOutputFilePath;
        }

        if (coverageFiles.Any())
        {
            var settings = new ReportGeneratorSettings();
            if (BuildParameters.IsDotNetCoreBuild && BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
            {
                // Workaround until 0.38.5+ version of cake is released
                // https://github.com/cake-build/cake/pull/2824
                settings.ToolPath = Context.Tools.Resolve("reportgenerator");
            }

            ReportGenerator(coverageFiles, BuildParameters.Paths.Directories.TestCoverage, settings);
        }
        else
        {
            Warning("No coverage files was found, no local report is generated!");
        }
    })
);

BuildParameters.Tasks.TestTask = Task("Test");
