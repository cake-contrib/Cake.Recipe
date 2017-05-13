///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.UploadCoverageReportTask = Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .IsDependentOn("Test")
    .Does(() => RequireTool(CoverallsTool, () => {
        CoverallsIo(BuildParameters.Paths.Files.TestCoverageOutputFilePath, new CoverallsIoSettings()
        {
            RepoToken = BuildParameters.Coveralls.RepoToken
        });
    })
);