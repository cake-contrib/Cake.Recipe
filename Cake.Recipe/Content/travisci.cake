BuildParameters.Tasks.PrintTravisCIEnvironmentVariablesTask = Task("Print-TravisCI-Environment-Variables")
    .WithCriteria(() => TravisCI.IsRunningOnTravisCI)
    .Does(() =>
    {
        using(var travisCI = TravisCI.Fold("Environment Variables"))
        {
            Information("CI: {0}", EnvironmentVariable("CI"));
            Information("TRAVIS_BRANCH: {0}", EnvironmentVariable("TRAVIS_BRANCH"));
            Information("TRAVIS_BUILD_DIR: {0}", EnvironmentVariable("TRAVIS_BUILD_DIR"));
            Information("TRAVIS_BUILD_ID: {0}", EnvironmentVariable("TRAVIS_BUILD_ID"));
            Information("TRAVIS_BUILD_NUMBER: {0}", EnvironmentVariable("TRAVIS_BUILD_NUMBER"));
            Information("TRAVIS_COMMIT: {0}", EnvironmentVariable("TRAVIS_COMMIT"));
            Information("TRAVIS_COMMIT_RANGE: {0}", EnvironmentVariable("TRAVIS_COMMIT_RANGE"));
            Information("TRAVIS_JOB_ID: {0}", EnvironmentVariable("TRAVIS_JOB_ID"));
            Information("TRAVIS_JOB_NUMBER: {0}", EnvironmentVariable("TRAVIS_JOB_NUMBER"));
            Information("TRAVIS_OS_NAME: {0}", EnvironmentVariable("TRAVIS_OS_NAME"));
            Information("TRAVIS_PULL_REQUEST: {0}", EnvironmentVariable("TRAVIS_PULL_REQUEST"));
            Information("TRAVIS_REPO_SLUG: {0}", EnvironmentVariable("TRAVIS_REPO_SLUG"));
        }
    });

BuildParameters.Tasks.UploadTravisCIArtifactsTask = Task("Upload-TravisCI-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.IsRunningOnTravisCI)
    .WithCriteria(() => DirectoryExists(BuildParameters.Directories.NuGetPackages))
    .Does(() =>
    {    
        using(var travisCI = TravisCI.Fold("Upload"))
        // Concatenating FilePathCollections should make sure we get unique FilePaths
        foreach(var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                            GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*"))
        {
            TravisCI.UploadArtifact(package);
        }
    });

BuildParameters.Task.ClearTravisCICacheTask = Task("Clear-TravisCI-Cache")
    .Does(() => 
    {

    });