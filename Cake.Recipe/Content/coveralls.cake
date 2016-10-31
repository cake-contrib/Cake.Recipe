///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(parameters.Paths.Files.TestCoverageOutputFilePath))
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMainRepository)
    .IsDependentOn("Test")
    .Does(() =>
{
    CoverallsIo(parameters.Paths.Files.TestCoverageOutputFilePath, new CoverallsIoSettings()
    {
        RepoToken = parameters.Coveralls.RepoToken
    });
});