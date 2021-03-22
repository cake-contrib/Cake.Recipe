///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
BuildParameters.Tasks.DupFinderTask = Task("DupFinder")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunDupFinder, "Skipping because DupFinder has been disabled")
    .Does(() => RequireTool(ToolSettings.ReSharperTools, () => {
        var dupFinderLogFilePath = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml");

        var settings = new DupFinderSettings() {
            ShowStats = true,
            ShowText = true,
            OutputFile = dupFinderLogFilePath,
            ExcludeCodeRegionsByNameSubstring = new string [] { "DupFinder Exclusion" },
            ThrowExceptionOnFindingDuplicates = ToolSettings.DupFinderThrowExceptionOnFindingDuplicates ?? true
        };

        if (ToolSettings.DupFinderExcludePattern != null)
        {
            settings.ExcludePattern = ToolSettings.DupFinderExcludePattern;
        }

        if (ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring != null)
        {
            settings.ExcludeFilesByStartingCommentSubstring = ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring;
        }

        if (ToolSettings.DupFinderDiscardCost != null)
        {
            settings.DiscardCost = ToolSettings.DupFinderDiscardCost.Value;
        }

        DupFinder(BuildParameters.SolutionFilePath, settings);

        // Pass path to dupFinder log file to Cake.Issues.Recipe
        IssuesParameters.InputFiles.DupFinderLogFilePath = dupFinderLogFilePath;
    })
);

BuildParameters.Tasks.InspectCodeTask = Task("InspectCode")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunInspectCode, "Skipping because InspectCode has been disabled")
    .Does<BuildData>(data => RequireTool(ToolSettings.ReSharperTools, () => {
        var inspectCodeLogFilePath = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml");

        var settings = new InspectCodeSettings() {
            SolutionWideAnalysis = true,
            OutputFile = inspectCodeLogFilePath
        };

        if (FileExists(BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName)))
        {
            settings.Profile = BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName);
        }

        InspectCode(BuildParameters.SolutionFilePath, settings);

        // Pass path to InspectCode log file to Cake.Issues.Recipe
        IssuesParameters.InputFiles.InspectCodeLogFilePath = inspectCodeLogFilePath;
    })
);

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode");
