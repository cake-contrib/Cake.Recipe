///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.PrintAppVeyorEnvironmentVariablesTask = Task("Print-AppVeyor-Environment-Variables")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor, "Skipping because not running an AppVeyor")
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

BuildParameters.Tasks.ClearAppVeyorCacheTask = Task("Clear-AppVeyor-Cache")
    .Does(() =>
        RequireAddin(@"#addin nuget:?package=Cake.AppVeyor&version=4.0.0&loaddependencies=true
        AppVeyorClearCache(new AppVeyorSettings() { ApiToken = EnvironmentVariable(""TEMP_APPVEYOR_TOKEN"") },
            EnvironmentVariable(""TEMP_APPVEYOR_ACCOUNT_NAME""),
            EnvironmentVariable(""TEMP_APPVEYOR_PROJECT_SLUG""));
        ",
        new Dictionary<string, string> {{"TEMP_APPVEYOR_TOKEN", BuildParameters.AppVeyor.ApiToken},
            {"TEMP_APPVEYOR_ACCOUNT_NAME", BuildParameters.AppVeyorAccountName},
            {"TEMP_APPVEYOR_PROJECT_SLUG", BuildParameters.AppVeyorProjectSlug}}
));

///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class AppVeyorTagInfo : ITagInfo
{
    public AppVeyorTagInfo(IAppVeyorProvider appVeyor)
    {
        IsTag = appVeyor.Environment.Repository.Tag.IsTag;
        Name = appVeyor.Environment.Repository.Tag.Name;
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class AppVeyorRepositoryInfo : IRepositoryInfo
{
    public AppVeyorRepositoryInfo(IAppVeyorProvider appVeyor)
    {
        Branch = appVeyor.Environment.Repository.Branch;
        Name = appVeyor.Environment.Repository.Name;
        Tag = new AppVeyorTagInfo(appVeyor);
    }

    public string Branch { get; }

    public string Name { get; }

    public ITagInfo Tag { get; }
}

public class AppVeyorPullRequestInfo : IPullRequestInfo
{
    public AppVeyorPullRequestInfo(IAppVeyorProvider appVeyor)
    {
        IsPullRequest = appVeyor.Environment.PullRequest.IsPullRequest;
    }

    public bool IsPullRequest { get; }
}

public class AppVeyorBuildInfo : IBuildInfo
{
    public AppVeyorBuildInfo(IAppVeyorProvider appVeyor)
    {
        Number = appVeyor.Environment.Build.Number.ToString();
    }

    public string Number { get; }
}

public class AppVeyorBuildProvider : IBuildProvider
{
    public AppVeyorBuildProvider(IAppVeyorProvider appVeyor)
    {
        Repository = new AppVeyorRepositoryInfo(appVeyor);
        PullRequest = new AppVeyorPullRequestInfo(appVeyor);
        Build = new AppVeyorBuildInfo(appVeyor);

        _appVeyor = appVeyor;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    private readonly IAppVeyorProvider _appVeyor;

    public void UploadArtifact(FilePath file)
    {
        _appVeyor.UploadArtifact(file);    
    }
}
