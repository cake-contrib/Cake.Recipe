///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.UploadCoverallsReportTask = Task("Upload-Coveralls-Report")
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .Does(() => RequireTool(ToolSettings.CoverallsTool, () => {
        if (BuildParameters.CanPublishToCoveralls)
        {
            var coverageFiles = GetFiles(BuildParameters.Paths.Directories.TestCoverage + "/coverlet/*.xml");
            if (FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath))
            {
                coverageFiles += BuildParameters.Paths.Files.TestCoverageOutputFilePath;
            }

            foreach(var coverageFile in coverageFiles)
            {
                CoverallsIo(coverageFile, new CoverallsIoSettings()
                {
                    RepoToken = BuildParameters.Coveralls.RepoToken
                });
            }
        }
        else
        {
            Warning("Unable to publish to Coveralls, as necessary credentials are not available");
        }
    })
).OnError (exception =>
{
    Error(exception.Message);
    Information("Upload-Coveralls-Report Task failed, but continuing with next Task...");
    publishingError = true;
});
