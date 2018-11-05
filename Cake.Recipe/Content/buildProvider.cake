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
        Number = appVeyor.Environment.Build.Number;
    }

    public int Number { get; }    
}

public class AppVeyorBuildProvider : IBuildProvider
{
    public AppVeyorBuildProvider(IAppVeyorProvider appVeyor)
    {
        Repository = new AppVeyorRepositoryInfo(appVeyor);
        PullRequest = new AppVeyorPullRequestInfo(appVeyor);
        Build = new AppVeyorBuildInfo(appVeyor);
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }
}

public static IBuildProvider GetBuildProvider(ICakeContext context, BuildSystem buildSystem)
{
    // always fallback to AppVeyor
    return new AppVeyorBuildProvider(buildSystem.AppVeyor);
}