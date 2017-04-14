///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Install-ReportGenerator")
    .Does(() => RequireTool(ReportGeneratorTool, () => {
    }));

Task("Install-ReportUnit")
    .IsDependentOn("Install-ReportGenerator")
    .Does(() => RequireTool(ReportUnitTool, () => {
    }));

Task("Install-OpenCover")
    .IsDependentOn("Install-ReportUnit")
    .Does(() => RequireTool(OpenCoverTool, () => {
    }));

var testNUnitTask = Task("Test-NUnit")
    .IsDependentOn("Install-OpenCover")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedNUnitTests))
    .Does(() => RequireTool(NUnitTool, () => {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.NUnitTestResults);

        OpenCover(tool => {
            tool.NUnit3(GetFiles(BuildParameters.Paths.Directories.PublishedNUnitTests + "/**/*.Tests.dll"), new NUnit3Settings {
                NoResults = true
            });
        },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
    })
);

var testxUnitTask = Task("Test-xUnit")
    .IsDependentOn("Install-OpenCover")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedxUnitTests))
    .Does(() => RequireTool(XUnitTool, () => {
    EnsureDirectoryExists(BuildParameters.Paths.Directories.xUnitTestResults);

        OpenCover(tool => {
            tool.XUnit2(GetFiles(BuildParameters.Paths.Directories.PublishedxUnitTests + "/**/*.Tests.dll"), new XUnit2Settings {
                OutputDirectory = BuildParameters.Paths.Directories.xUnitTestResults,
                XmlReport = true,
                NoAppDomain = true
            });
        },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportUnit(BuildParameters.Paths.Directories.xUnitTestResults, BuildParameters.Paths.Directories.xUnitTestResults, new ReportUnitSettings());

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
    })
);

var testMSTestTask = Task("Test-MSTest")
    .IsDependentOn("Install-OpenCover")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedMSTestTests))
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.MSTestTestResults);

    // TODO: Need to add OpenCover here
    MSTest(GetFiles(BuildParameters.Paths.Directories.PublishedMSTestTests + "/**/*.Tests.dll"), new MSTestSettings() {
        NoIsolation = false
    });
});

var testVSTestTask = Task("Test-VSTest")
    .IsDependentOn("Install-OpenCover")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedVSTestTests))
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.Paths.Directories.VSTestTestResults);

    var vsTestSettings = new VSTestSettings()
    {
        InIsolation = true
    };

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        vsTestSettings.WithAppVeyorLogger();
    } 

    OpenCover(
		tool => { tool.VSTest(GetFiles(BuildParameters.Paths.Directories.PublishedVSTestTests + "/**/*.Tests.dll"), vsTestSettings); },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings() { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportUnit(BuildParameters.Paths.Directories.VSTestTestResults, BuildParameters.Paths.Directories.VSTestTestResults, new ReportUnitSettings());

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
});

var testFixieTask = Task("Test-Fixie")
    .IsDependentOn("Install-OpenCover")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PublishedFixieTests))
    .Does(() => RequireTool(FixieTool, () => {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.FixieTestResults);

        OpenCover(tool => {
            tool.Fixie(GetFiles(BuildParameters.Paths.Directories.PublishedFixieTests + "/**/*.Tests.dll"), new FixieSettings  {           
                XUnitXml = BuildParameters.Paths.Directories.FixieTestResults + "TestResult.xml"
            });
        },
        BuildParameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
            .WithFilter(ToolSettings.TestCoverageFilter)
            .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
            .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportUnit(BuildParameters.Paths.Directories.FixieTestResults, BuildParameters.Paths.Directories.FixieTestResults, new ReportUnitSettings());

        // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
        ReportGenerator(BuildParameters.Paths.Files.TestCoverageOutputFilePath, BuildParameters.Paths.Directories.TestCoverage);
    })
);

var testTask = Task("Test")
    .IsDependentOn("Test-NUnit")
    .IsDependentOn("Test-xUnit")
    .IsDependentOn("Test-MSTest")
    .IsDependentOn("Test-VSTest")
    .IsDependentOn("Test-Fixie")
    .Does(() =>
{
});
