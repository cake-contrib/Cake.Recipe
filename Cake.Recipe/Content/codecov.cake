///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.UploadCodecovReportTask = Task("Upload-Codecov-Report")
    .WithCriteria(() => FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .WithCriteria(() => BuildParameters.CanPublishToCodecov)
    .Does(() => RequireTool(CodecovTool, () => {
        var settings = new CodecovSettings {
            Files = new[] { BuildParameters.Paths.Files.TestCoverageOutputFilePath.ToString() },
            Token = BuildParameters.Codecov.RepoToken
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
);