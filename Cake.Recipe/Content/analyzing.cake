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
    .WithCriteria(() => BuildParameters.IsRunningOnWindows, "Skipping due to not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunDupFinder, "Skipping because DupFinder has been disabled")
    .Does(() => RequireTool(ToolSettings.ReSharperTools, () => {
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
    RequireTool(ToolSettings.ReSharperReportsTool, () => {
        var outputHtmlFile = BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.html");

        Information("Duplicates were found in your codebase, creating HTML report...");
        ReSharperReports(
            BuildParameters.Paths.Directories.DupFinderTestResults.CombineWithFilePath("dupfinder.xml"),
            outputHtmlFile);

        if(!BuildParameters.IsLocalBuild && FileExists(outputHtmlFile))
        {
            BuildParameters.BuildProvider.UploadArtifact(outputHtmlFile);
        }

        if(BuildParameters.IsLocalBuild)
        {
            LaunchDefaultProgram(outputHtmlFile);
        }
    });
});

BuildParameters.Tasks.InspectCodeTask = Task("InspectCode")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows, "Skipping due to not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldRunInspectCode, "Skipping because InspectCode has been disabled")
    .Does<BuildData>(data => RequireTool(ToolSettings.ReSharperTools, () => {
        var inspectCodeLogFilePath = BuildParameters.Paths.Directories.InspectCodeTestResults.CombineWithFilePath("inspectcode.xml");

        var settings = new InspectCodeSettings() {
            SolutionWideAnalysis = true,
            OutputFile = inspectCodeLogFilePath
        };

        if(FileExists(BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName)))
        {
            settings.Profile = BuildParameters.SourceDirectoryPath.CombineWithFilePath(BuildParameters.ResharperSettingsFileName);
        }

        InspectCode(BuildParameters.SolutionFilePath, settings);

        // Parse issues.
        var issues =
            ReadIssues(
                InspectCodeIssuesFromFilePath(inspectCodeLogFilePath),
                data.RepositoryRoot);
        Information("{0} InspectCode issues are found.", issues.Count());
        data.AddIssues(issues);
    })
);

BuildParameters.Tasks.CreateIssuesReportTask = Task("CreateIssuesReport")
    .IsDependentOn("InspectCode")
    .Does<BuildData>(data => {
        var issueReportFile = BuildParameters.Paths.Directories.TestResults.CombineWithFilePath("issues-report.html");

        CreateIssueReport(
            data.Issues,
            GenericIssueReportFormatFromEmbeddedTemplate(GenericIssueReportTemplate.HtmlDxDataGrid),
            "./",
            issueReportFile);

        if(!BuildParameters.IsLocalBuild && FileExists(issueReportFile))
        {
            BuildParameters.BuildProvider.UploadArtifact(issueReportFile);
        }
    });

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("DupFinder")
    .IsDependentOn("InspectCode")
    .IsDependentOn("CreateIssuesReport");
