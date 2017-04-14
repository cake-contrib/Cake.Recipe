///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

var printAppVeyorEnviromentVariablesTask = Task("Print-AppVeyor-Environment-Variables")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    Information("CI: {0}", EnvironmentVariable("CI"));
    Information("APPVEYOR_API_URL: {0}", EnvironmentVariable("APPVEYOR_API_URL"));
    Information("APPVEYOR_PROJECT_ID: {0}", EnvironmentVariable("APPVEYOR_PROJECT_ID"));
    Information("APPVEYOR_PROJECT_NAME: {0}", EnvironmentVariable("APPVEYOR_PROJECT_NAME"));
    Information("APPVEYOR_PROJECT_SLUG: {0}", EnvironmentVariable("APPVEYOR_PROJECT_SLUG"));
    Information("APPVEYOR_BUILD_FOLDER: {0}", EnvironmentVariable("APPVEYOR_BUILD_FOLDER"));
    Information("APPVEYOR_BUILD_ID: {0}", EnvironmentVariable("APPVEYOR_BUILD_ID"));
    Information("APPVEYOR_BUILD_NUMBER: {0}", EnvironmentVariable("APPVEYOR_BUILD_NUMBER"));
    Information("APPVEYOR_BUILD_VERSION: {0}", EnvironmentVariable("APPVEYOR_BUILD_VERSION"));
    Information("APPVEYOR_PULL_REQUEST_NUMBER: {0}", EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
    Information("APPVEYOR_PULL_REQUEST_TITLE: {0}", EnvironmentVariable("APPVEYOR_PULL_REQUEST_TITLE"));
    Information("APPVEYOR_JOB_ID: {0}", EnvironmentVariable("APPVEYOR_JOB_ID"));
    Information("APPVEYOR_REPO_PROVIDER: {0}", EnvironmentVariable("APPVEYOR_REPO_PROVIDER"));
    Information("APPVEYOR_REPO_SCM: {0}", EnvironmentVariable("APPVEYOR_REPO_SCM"));
    Information("APPVEYOR_REPO_NAME: {0}", EnvironmentVariable("APPVEYOR_REPO_NAME"));
    Information("APPVEYOR_REPO_BRANCH: {0}", EnvironmentVariable("APPVEYOR_REPO_BRANCH"));
    Information("APPVEYOR_REPO_TAG: {0}", EnvironmentVariable("APPVEYOR_REPO_TAG"));
    Information("APPVEYOR_REPO_TAG_NAME: {0}", EnvironmentVariable("APPVEYOR_REPO_TAG_NAME"));
    Information("APPVEYOR_REPO_COMMIT: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT"));
    Information("APPVEYOR_REPO_COMMIT_AUTHOR: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"));
    Information("APPVEYOR_REPO_COMMIT_TIMESTAMP: {0}", EnvironmentVariable("APPVEYOR_REPO_COMMIT_TIMESTAMP"));
    Information("APPVEYOR_SCHEDULED_BUILD: {0}", EnvironmentVariable("APPVEYOR_SCHEDULED_BUILD"));
    Information("APPVEYOR_FORCED_BUILD: {0}", EnvironmentVariable("APPVEYOR_FORCED_BUILD"));
    Information("APPVEYOR_RE_BUILD: {0}", EnvironmentVariable("APPVEYOR_RE_BUILD"));
    Information("PLATFORM: {0}", EnvironmentVariable("PLATFORM"));
    Information("CONFIGURATION: {0}", EnvironmentVariable("CONFIGURATION"));
});

var uploadAppVeyorArtifactsTask = Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.IsRunningOnAppVeyor)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages) || DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    foreach(var package in GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*"))
    {
        AppVeyor.UploadArtifact(package);
    }

    foreach(var package in GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
    {
        AppVeyor.UploadArtifact(package);
    }
});

var clearAppVeyorCacheTask = Task("Clear-AppVeyor-Cache")
    .Does(() => 
        RequireAddin(@"#addin nuget:?package=Cake.AppVeyor&version=1.1.0.9
        AppVeyorClearCache(new AppVeyorSettings() { ApiToken = EnvironmentVariable(""TEMP_APPVEYOR_TOKEN"") }, 
            EnvironmentVariable(""TEMP_APPVEYOR_ACCOUNT_NAME""), 
            EnvironmentVariable(""TEMP_APPVEYOR_PROJECT_SLUG""));
        ",
        new Dictionary<string, string> {{"TEMP_APPVEYOR_TOKEN", BuildParameters.AppVeyor.ApiToken}, 
            {"TEMP_APPVEYOR_ACCOUNT_NAME", BuildParameters.AppVeyorAccountName}, 
            {"TEMP_APPVEYOR_PROJECT_SLUG", BuildParameters.AppVeyorProjectSlug}}
));