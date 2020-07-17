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

    public bool SupportsTokenlessCodecov { get; } = true;

    public IEnumerable<string> PrintVariables { get; } = new[] {
        "CI",
        "TRAVIS",
        "TRAVIS_BRANCH",
        "TRAVIS_BUILD_DIR",
        "TRAVIS_BUILD_ID",
        "TRAVIS_BUILD_NUMBER",
        "TRAVIS_COMMIT",
        "TRAVIS_COMMIT_MESSAGE",
        "TRAVIS_COMMIT_RANGE",
        "TRAVIS_CPU_ARCH",
        "TRAVIS_DIST",
        "TRAVIS_JOB_ID",
        "TRAVIS_JOB_NAME",
        "TRAVIS_JOB_NUMBER",
        "TRAVIS_JOB_WEB_URL",
        "TRAVIS_OS_NAME",
        "TRAVIS_OSX_IMAGE",
        "TRAVIS_PROJECT_SLUG",
        "TRAVIS_PULL_REQUEST",
        "TRAVIS_PULL_REQUEST_BRANCH",
        "TRAVIS_PULL_REQUEST_SHA",
        "TRAVIS_PULL_REQUEST_SLUG",
        "TRAVIS_TAG"
    };

    public void UploadArtifact(FilePath file)
    {
        _context.Information("Unable to upload artifacts as running on TRAVIS CI");
    }
}