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
    string Number { get; }
}

public interface IBuildProvider
{
    IRepositoryInfo Repository { get; }

    IPullRequestInfo PullRequest { get; }

    IBuildInfo Build { get; }
}

public static IBuildProvider GetBuildProvider(ICakeContext context, BuildSystem buildSystem)
{
    //todo: need to be replaced to `IsRunningOnAzurePipelines || IsRunningOnAzurePipelinesHosted` after update to Cake 0.33.0
    if (buildSystem.IsRunningOnTFS || buildSystem.IsRunningOnVSTS)
    {
        return new AzurePipelinesBuildProvider(buildSystem.TFBuild);
    }

    // always fallback to AppVeyor
    return new AppVeyorBuildProvider(buildSystem.AppVeyor);
}