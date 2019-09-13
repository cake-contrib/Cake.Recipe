public interface ITagInfo
{
    bool IsTag { get; }

    string Name { get; }
}

public interface IRepositoryInfo
{
    string Branch { get; }

    string Name { get; }

    ITagInfo Tag { get; }
}

public interface IPullRequestInfo
{
    bool IsPullRequest { get; }
}

public interface IBuildInfo
{
    int Number { get; }
}

public interface IBuildProvider
{
    IRepositoryInfo Repository { get; }

    IPullRequestInfo PullRequest { get; }

    IBuildInfo Build { get; }
}

public static IBuildProvider GetBuildProvider(ICakeContext context, BuildSystem buildSystem)
{
    // always fallback to AppVeyor
    return new AppVeyorBuildProvider(buildSystem.AppVeyor);
}
