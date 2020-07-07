BuildParameters.Tasks.PrintTravisCiEnvironmentVariablesTask = Task("Print-Travis-Ci-Environment-Variables")
    .WithCriteria(() => TravisCI.IsRunningOnTravisCI, "Skipping because not running on Travis CI")
    .Does(() =>
{
    Information("CI: {0}", EnvironmentVariable("CI"));
    Information("TRAVIS: {0}", EnvironmentVariable("TRAVIS"));
    Information("TRAVIS_BRANCH: {0}", EnvironmentVariable("TTRAVIS_BRANCH"));
    Information("TRAVIS_BUILD_DIR: {0}", EnvironmentVariable("TRAVIS_BUILD_DIR"));
    Information("TRAVIS_BUILD_ID: {0}", EnvironmentVariable("TRAVIS_BUILD_ID"));
    Information("TRAVIS_BUILD_NUMBER: {0}", EnvironmentVariable("TRAVIS_BUILD_NUMBER"));
    Information("TRAVIS_COMMIT: {0}", EnvironmentVariable("TRAVIS_COMMIT"));
    Information("TRAVIS_COMMIT_MESSAGE: {0}", EnvironmentVariable("TRAVIS_COMMIT_MESSAGE"));
    Information("TRAVIS_COMMIT_RANGE: {0}", EnvironmentVariable("TRAVIS_COMMIT_RANGE"));
    Information("TRAVIS_CPU_ARCH: {0}", EnvironmentVariable("TRAVIS_CPU_ARCH"));
    Information("TRAVIS_DIST: {0}", EnvironmentVariable("TRAVIS_DIST"));
    Information("TRAVIS_JOB_ID: {0}", EnvironmentVariable("TRAVIS_JOB_ID"));
    Information("TRAVIS_JOB_NAME: {0}", EnvironmentVariable("TRAVIS_JOB_NAME"));
    Information("TRAVIS_JOB_NUMBER: {0}", EnvironmentVariable("TRAVIS_JOB_NUMBER"));
    Information("TRAVIS_JOB_WEB_URL: {0}", EnvironmentVariable("TRAVIS_JOB_WEB_URL"));
    Information("TRAVIS_OS_NAME: {0}", EnvironmentVariable("TRAVIS_OS_NAME"));
    Information("TRAVIS_OSX_IMAGE: {0}", EnvironmentVariable("TRAVIS_OSX_IMAGE"));
    Information("TRAVIS_PROJECT_SLUG: {0}", EnvironmentVariable("TRAVIS_PROJECT_SLUG"));
    Information("TRAVIS_PULL_REQUEST: {0}", EnvironmentVariable("TRAVIS_PULL_REQUEST"));
    Information("TRAVIS_PULL_REQUEST_BRANCH: {0}", EnvironmentVariable("TRAVIS_PULL_REQUEST_BRANCH"));
    Information("TRAVIS_PULL_REQUEST_SHA: {0}", EnvironmentVariable("TRAVIS_PULL_REQUEST_SHA"));
    Information("TRAVIS_PULL_REQUEST_SLUG: {0}", EnvironmentVariable("TRAVIS_PULL_REQUEST_SLUG"));
    Information("TRAVIS_TAG: {0}", EnvironmentVariable("TRAVIS_TAG"));
});

public class TravisCiTagInfo : ITagInfo
{
    public TravisCiTagInfo(ITravisCIProvider travisCi)
    {
        Name = travisCi.Environment.Build.Tag;
    }

    public bool IsTag => !string.IsNullOrEmpty(Name);
    public string Name { get; }
}

public class TravisCiRepositoryInfo : IRepositoryInfo
{
    public TravisCiRepositoryInfo(ITravisCIProvider travisCi)
    {
        Branch = travisCi.Environment.Build.Branch;
        Name = travisCi.Environment.Repository.Slug;
        Tag = new TravisCiTagInfo(travisCi);
    }

    public string Branch { get; }
    public string Name { get; }
    public ITagInfo Tag { get; }
}

public class TravisCiPullRequestInfo : IPullRequestInfo
{
    public TravisCiPullRequestInfo(ITravisCIProvider travisCi)
    {
        IsPullRequest = travisCi.Environment.PullRequest.IsPullRequest;
    }

    public bool IsPullRequest { get; }
}

public class TravisCiBuildInfo : IBuildInfo
{
    public TravisCiBuildInfo(ITravisCIProvider travisCi)
    {
        Number = travisCi.Environment.Build.BuildNumber.ToString();
    }

    public string Number { get; }
}

public class TravisCiBuildProvider : IBuildProvider
{
    private readonly ICakeContext _context;

    public TravisCiBuildProvider(ITravisCIProvider travisCi, ICakeContext context)
    {
        Build = new TravisCiBuildInfo(travisCi);
        PullRequest = new TravisCiPullRequestInfo(travisCi);
        Repository = new TravisCiRepositoryInfo(travisCi);

        _context = context;
    }

    public IBuildInfo Build { get; }
    public IPullRequestInfo PullRequest { get; }
    public IRepositoryInfo Repository { get; }

    public void UploadArtifact(FilePath file)
    {
        _context.Information("Unable to upload artifacts as running on TRAVIS CI");
    }
}