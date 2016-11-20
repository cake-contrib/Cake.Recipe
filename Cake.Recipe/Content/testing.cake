///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console&version=2.1.0"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.4.1"
#tool "nuget:?package=OpenCover&version=4.6.519"
#tool "nuget:?package=ReportGenerator&version=2.4.5"
#tool "nuget:?package=ReportUnit&version=1.2.1"

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Test-NUnit")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.PublishedNUnitTests))
    .Does(() =>
{
    EnsureDirectoryExists(parameters.Paths.Directories.NUnitTestResults);

    OpenCover(tool => {
        tool.NUnit3(GetFiles(parameters.Paths.Directories.PublishedNUnitTests + "/**/*.Tests.dll"), new NUnit3Settings {
            NoResults = true
        });
    },
    parameters.Paths.Files.TestCoverageOutputFilePath,
    new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
        .WithFilter(testCoverageFilter)
        .ExcludeByAttribute(testCoverageExcludeByAttribute)
        .ExcludeByFile(testCoverageExcludeByFile));

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportGenerator(parameters.Paths.Files.TestCoverageOutputFilePath, parameters.Paths.Directories.TestCoverage);
});

Task("Test-xUnit")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.PublishedxUnitTests))
    .Does(() =>
{
    EnsureDirectoryExists(parameters.Paths.Directories.xUnitTestResults);

    OpenCover(tool => {
        tool.XUnit2(GetFiles(parameters.Paths.Directories.PublishedxUnitTests + "/**/*.Tests.dll"), new XUnit2Settings {
            OutputDirectory = parameters.Paths.Directories.xUnitTestResults,
            XmlReport = true,
            NoAppDomain = true
        });
    },
    parameters.Paths.Files.TestCoverageOutputFilePath,
    new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
        .WithFilter(testCoverageFilter)
        .ExcludeByAttribute(testCoverageExcludeByAttribute)
        .ExcludeByFile(testCoverageExcludeByFile));

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportUnit(parameters.Paths.Directories.xUnitTestResults, parameters.Paths.Directories.xUnitTestResults, new ReportUnitSettings());

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportGenerator(parameters.Paths.Files.TestCoverageOutputFilePath, parameters.Paths.Directories.TestCoverage);
});

Task("Test-MSTest")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.PublishedMSTestTests))
    .Does(() =>
{
    EnsureDirectoryExists(parameters.Paths.Directories.MSTestTestResults);

    // TODO: Need to add OpenCover here
    MSTest(GetFiles(parameters.Paths.Directories.PublishedMSTestTests + "/**/*.Tests.dll"), new MSTestSettings() {
        NoIsolation = false
    });
});

Task("Test-VSTest")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.PublishedVSTestTests))
    .Does(() =>
{
    EnsureDirectoryExists(parameters.Paths.Directories.VSTestTestResults);

    var vsTestSettings = new VSTestSettings()
    {
        InIsolation = true
    };

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        vsTestSettings.WithAppVeyorLogger();
    } 

    OpenCover(
		tool => { tool.VSTest(GetFiles(parameters.Paths.Directories.PublishedVSTestTests + "/**/*.Tests.dll"), vsTestSettings); },
        parameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings() { ReturnTargetCodeOffset = 0 }
            .WithFilter(testCoverageFilter)
            .ExcludeByAttribute(testCoverageExcludeByAttribute)
            .ExcludeByFile(testCoverageExcludeByFile));

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportUnit(parameters.Paths.Directories.VSTestTestResults, parameters.Paths.Directories.VSTestTestResults, new ReportUnitSettings());

    // TODO: Need to think about how to bring this out in a generic way for all Test Frameworks
    ReportGenerator(parameters.Paths.Files.TestCoverageOutputFilePath, parameters.Paths.Directories.TestCoverage);
});

Task("Test")
    .IsDependentOn("Test-NUnit")
    .IsDependentOn("Test-xUnit")
    .IsDependentOn("Test-MSTest")
    .IsDependentOn("Test-VSTest")
    .Does(() =>
{
});
