///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.UploadCoverageReportTask = Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .Does(() => RequireTool(CoverallsTool, () => {
        if(BuildParameters.CanPublishToCoveralls)
        {
            CoverallsIo(BuildParameters.Paths.Files.TestCoverageOutputFilePath, new CoverallsIoSettings()
            {
                RepoToken = BuildParameters.Coveralls.RepoToken
            });
        }
        else
        {
            Warning("Unable to publish to Coveralls, as necessary credentials are not available");
        }
    })
);