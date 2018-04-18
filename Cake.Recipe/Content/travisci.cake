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
        {
            TravisCIUpload(settings => 
            {
                settings.AuthToken = EnvironmentVariable("AuthToken");
                settings.TargetPaths = new[] 
                {
                    GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*"), 
                    GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*")
                };
            });
        }
    });

BuildParameters.Task.ClearTravisCICacheTask = Task("Clear-TravisCI-Cache")
    .Does(() =>         
        RequireAddin(@"#addin nuget:?package=Cake.TravisCI&version=0.1.0
        TravisCICache(new TravisCISettings() { ApiToken = EnvironmentVariable(""TEMP_TRAVISCI_TOKEN"") },
            EnvironmentVariable(""TEMP_TRAVISCI_ACCOUNT_NAME""),
            EnvironmentVariable(""TEMP_TRAVISCI_PROJECT_SLUG""));
        ",
        new Dictionary<string, string> {{"TEMP_TRAVISCI_TOKEN", BuildParameters.TravisCI.ApiToken},
            {"TEMP_TRAVISCI_ACCOUNT_NAME", BuildParameters.TravisCIAccountName},
            {"TEMP_TRAVISCI_PROJECT_SLUG", BuildParameters.TravisCIProjectSlug}}));