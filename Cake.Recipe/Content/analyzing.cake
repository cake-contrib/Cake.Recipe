///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
BuildParameters.Tasks.DupFinderTask = Task("DupFinder")
    .WithCriteria(() => BuildParameters.IsDotNetCoreBuild || BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows, or using .Net Global Tools for JetBrains")
    .WithCriteria(() => BuildParameters.ShouldRunDupFinder, "Skipping because DupFinder has been disabled")
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.ReSharperGlobalTools : ToolSettings.ReSharperTools, () => {
        var dupFinderLogFilePath = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml");

        var settings = new DupFinderSettings() {
            ShowStats = true,
            ShowText = true,
            OutputFile = dupFinderLogFilePath,
            ExcludeCodeRegionsByNameSubstring = new string [] { "DupFinder Exclusion" },
            ThrowExceptionOnFindingDuplicates = ToolSettings.DupFinderThrowExceptionOnFindingDuplicates ?? true
        };

        // Workaround until 1.0.0+ version of cake is released
        if (BuildParameters.IsDotNetCoreBuild && BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows)
        {
            settings.ToolPath = Context.Tools.Resolve("jb.exe");
            settings.ArgumentCustomization = args => args.Prepend("dupfinder");
        }
        else if(BuildParameters.IsDotNetCoreBuild)
        {
            settings.ToolPath = Context.Tools.Resolve("jb");
            settings.ArgumentCustomization = args => args.Prepend("dupfinder");
        }

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
    .WithCriteria(() => BuildParameters.IsDotNetCoreBuild || BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping due to not running on Windows, or using .Net Global Tools for JetBrains")
    .WithCriteria(() => BuildParameters.ShouldRunInspectCode, "Skipping because InspectCode has been disabled")
    .Does<BuildData>(data => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.ReSharperGlobalTools : ToolSettings.ReSharperTools, () => {
        var inspectCodeLogFilePath = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml");

        var settings = new InspectCodeSettings() {
            SolutionWideAnalysis = true,
            OutputFile = inspectCodeLogFilePath
        };

        // Workaround until 1.0.0+ version of cake is released
        if (BuildParameters.IsDotNetCoreBuild && BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows)
        {
            settings.ToolPath = Context.Tools.Resolve("jb.exe");
            settings.ArgumentCustomization = args => args.Prepend("inspectcode");
        }
        else if(BuildParameters.IsDotNetCoreBuild)
        {
            settings.ToolPath = Context.Tools.Resolve("jb");
            settings.ArgumentCustomization = args => args.Prepend("inspectcode");
        }

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
