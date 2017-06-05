///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////
using System.Diagnostics;

public void LaunchDefaultProgram(FilePath file) {
    FilePath program;
    string arguments = "";

    if (BuildParameters.IsRunningOnWindows)
    {
        program = "cmd";
        arguments = "/c start ";
    }
    else if ((program = Context.Tools.Resolve("xdg-open")) == null &&
             (program = Context.Tools.Resolve("open")) == null)
    {
        Warning("Unable to open report file: {0}", file.ToString());
        return;
    }

    arguments += " " + file.FullPath;
    // We can't use the StartProcess alias as this won't actually open the file.
    Process.Start(new ProcessStartInfo(program.FullPath, arguments) { CreateNoWindow = true });
}

BuildParameters.Tasks.DupFinderTask = Task("DupFinder")
    .Does(() => RequireTool(ReSharperTools, () => {
        var settings = new DupFinderSettings() {
            ShowStats = true,
            ShowText = true,
            OutputFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
            ExcludeCodeRegionsByNameSubstring = new string [] { "DupFinder Exclusion" },
            ThrowExceptionOnFindingDuplicates = ToolSettings.DupFinderThrowExceptionOnFindingDuplicates ?? true
        };

        if(ToolSettings.DupFinderExcludePattern != null)
        {
            settings.ExcludePattern = ToolSettings.DupFinderExcludePattern;
        }

        if(ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring != null)
        {
            settings.ExcludeFilesByStartingCommentSubstring = ToolSettings.DupFinderExcludeFilesByStartingCommentSubstring;
        }

        if(ToolSettings.DupFinderDiscardCost != null)
        {
            settings.DiscardCost = ToolSettings.DupFinderDiscardCost.Value;
        }

        DupFinder(BuildParameters.SolutionFilePath, settings);
    })
)
.ReportError(exception =>
{
    RequireTool(ReSharperReportsTool, () => {
        var outputHtmlFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.html");

        Information("Duplicates were found in your codebase, creating HTML report...");
        ReSharperReports(
            BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
            outputHtmlFile);

        if(BuildParameters.IsRunningOnAppVeyor && FileExists(outputHtmlFile))
        {
            AppVeyor.UploadArtifact(outputHtmlFile);
        }

        if(BuildParameters.IsLocalBuild)
        {
            LaunchDefaultProgram(outputHtmlFile);
        }
    });
});

BuildParameters.Tasks.InspectCodeTask = Task("InspectCode")
    .Does(() => RequireTool(ReSharperTools, () => {
        var settings = new InspectCodeSettings() {
            SolutionWideAnalysis = true,
            OutputFile = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
            ThrowExceptionOnFindingViolations = true
        };

        if(FileExists(BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName)))
        {
            settings.Profile = BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName);
        }

        InspectCode(BuildParameters.SolutionFilePath, settings);
    })
)
.ReportError(exception =>
{
    RequireTool(ReSharperReportsTool, () => {
        var outputHtmlFile = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.html");

        Information("Violations were found in your codebase, creating HTML report...");
        ReSharperReports(
            BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml"),
            outputHtmlFile);

        if(BuildParameters.IsRunningOnAppVeyor && FileExists(outputHtmlFile))
        {
            AppVeyor.UploadArtifact(outputHtmlFile);
        }

        if(BuildParameters.IsLocalBuild)
        {
            LaunchDefaultProgram(outputHtmlFile);
        }
    });
});

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode");
