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

    void UploadArtifact(FilePath file);
}

public static IBuildProvider GetBuildProvider(ICakeContext context, BuildSystem buildSystem)
{
    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
    {
        context.Information("Using Azure DevOps Pipelines Provider...");
        return new AzurePipelinesBuildProvider(buildSystem.TFBuild, context.Environment);
    }

    if (buildSystem.IsRunningOnTeamCity)
    {
        context.Information("Using TeamCity Provider...");
        return new TeamCityBuildProvider(buildSystem.TeamCity, context);
    }

    if (buildSystem.IsRunningOnAppVeyor)
    {
        context.Information("Using AppVeyor Provider...");
        return new AppVeyorBuildProvider(buildSystem.AppVeyor);
    }

    // always fallback to Local Build
    context.Information("Using Local Build Provider...");
    return new LocalBuildBuildProvider(context);
}
