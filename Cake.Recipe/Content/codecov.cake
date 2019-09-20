///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.UploadCodecovReportTask = Task("Upload-Codecov-Report")
    .WithCriteria(() => FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath), "Skipping because no coverage report has been generated")
    .WithCriteria(() => BuildParameters.IsMainRepository, "Skipping because not running from the main repository")
    .WithCriteria(() => BuildParameters.ShouldRunCodecov, "Skipping because uploading to codecov is disabled")
    .WithCriteria(() => BuildParameters.CanPublishToCodecov, "Skipping because repo token is missing, or not running on appveyor")
    .Does(() => RequireTool(ToolSettings.CodecovTool, () => {
        var settings = new CodecovSettings {
            Files = new[] { BuildParameters.Paths.Files.TestCoverageOutputFilePath.ToString() },
            Required = true
        };
        if (BuildParameters.Version != null &&
            !string.IsNullOrEmpty(BuildParameters.Version.FullSemVersion) &&
            BuildParameters.IsRunningOnAppVeyor)
        {
            // Required to work correctly with appveyor because environment changes isn't detected until cake is done running.
            var buildVersion = string.Format("{0}.build.{1}",
                BuildParameters.Version.FullSemVersion,
                BuildSystem.AppVeyor.Environment.Build.Number);
            settings.EnvironmentVariables = new Dictionary<string, string> { { "APPVEYOR_BUILD_VERSION", buildVersion }};
        }

        Codecov(settings);
    })
).OnError (exception =>
{
    Error(exception.Message);
    Information("Upload-Codecov-Report Task failed, but continuing with next Task...");
    publishingError = true;
});
