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

    DupFinder(BuildParameters.SolutionFilePath, settings);
})
.ReportError(exception =>
{
    var outputHtmlFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.html");

    Information("Duplicates were found in your codebase, creating HTML report...");
    ReSharperReports(
        BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
        outputHtmlFile);

    if(BuildParameters.IsRunningOnAppVeyor && FileExists(outputHtmlFile))
    {
        AppVeyor.UploadArtifact(outputHtmlFile);
    }

    if(BuildParameters.IsLocalBuild && BuildParameters.IsRunningOnWindows)
    {
        StartProcess("explorer.exe", outputHtmlFile);
    }
});

Task("InspectCode")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    InspectCode(BuildParameters.SolutionFilePath, new InspectCodeSettings() {
        SolutionWideAnalysis = true,
        Profile = BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName),
        OutputFile = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
        ThrowExceptionOnFindingViolations = true
    });
})
.ReportError(exception =>
{
    var outputHtmlFile = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.html");

    Information("Violations were found in your codebase, creating HTML report...");
    ReSharperReports(
        BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
        outputHtmlFile);

    if(BuildParameters.IsRunningOnAppVeyor && FileExists(outputHtmlFile))
    {
        AppVeyor.UploadArtifact(outputHtmlFile);
    }
    
    if(BuildParameters.IsLocalBuild && BuildParameters.IsRunningOnWindows)
    {
        StartProcess("explorer.exe", outputHtmlFile);
    }
});

Task("Analyze")
    .IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode");