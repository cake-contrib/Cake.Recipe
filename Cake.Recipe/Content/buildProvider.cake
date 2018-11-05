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

    bool IsTag { get; }

    string Name { get; }    
}

public class AppVeyorRepositoryInfo : IRepositoryInfo
{
    public AppVeyorRepositoryInfo(IAppVeyorProvider appVeyor)
    {
        Branch = appVeyor.Environment.Repository.Branch;
        Name = appVeyor.Environment.Repository.Name;
        Tag = new AppVeyorTagInfo(appVeyor);
    }

    string Branch { get; }

    string Name { get; }

    ITagInfo Tag { get; }    
}

public class AppVeyorPullRequestInfo : IPullRequestInfo
{
    public AppVeyorPulLRequestInfo(IAppVeyorProvider appVeyor)
    {
        IsPullRequest = appVeyor.Environment.PullRequest.IsPullRequest;
    }

    bool IsPullRequest { get; }    
}

public class AppVeyorBuildInfo : IBuildInfo
{
    public AppVeyorBuildInfo(IAppVeyorProvider appVeyor)
    {
        Number = appVeyor.Environment.Build.Number;
    }

    int Number { get; }    
}

public class AppVeyorBuildProvider : IBuildProvider
{
    public AppVeyorBuildProvider(IAppVeyorProvider appVeyor)
    {
        Repository = new AppVeyorRepositoryInfo(appVeyor);
        PullRequest = new AppVeyorPullRequestInfo(appVeyor);
        Build = new AppVeyorBuildInfo(appVeyor);
    }

    IRepositoryInfo Repository { get; }

    IPullRequestInfo PullRequest { get; }

    IBuildInfo Build { get; 
}

public IBuildProvider GetBuildProvider(ICakeContext context)
{
    if (context.BuildSystem.IsRunningOnAppVeyor)
    {
        return new AppVeyorBuildProvider(context.BuildSystem.AppVeyor);
    }
}