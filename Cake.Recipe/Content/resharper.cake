///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("DupFinder")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DupFinderSettings() {
        ShowStats = true,
        ShowText = true,
        OutputFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
        ThrowExceptionOnFindingDuplicates = true
    };

    if(ToolSettings.DupFinderExcludePattern != null)
    {
        settings.ExcludePattern = ToolSettings.DupFinderExcludePattern;
    }

    DupFinder(solutionFilePath, settings);
})
.ReportError(exception =>
{
    Information("Duplicates were found in your codebase, creating HTML report...");
    ReSharperReports(
        BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
        BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.html"));
});

Task("InspectCode")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    InspectCode(solutionFilePath, new InspectCodeSettings() {
        SolutionWideAnalysis = true,
        Profile = BuildParameters.Paths.Directories.Source.CombineWithFilePath(resharperSettingsFileName),
        OutputFile = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
        ThrowExceptionOnFindingViolations = true
    });
})
.ReportError(exception =>
{
    Information("Violations were found in your codebase, creating HTML report...");
    ReSharperReports(
        BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
        BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.html"));
});

Task("Analyze")
    .IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode");