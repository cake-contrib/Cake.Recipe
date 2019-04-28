///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class AzurePipelinesTagInfo : ITagInfo
{
    public AzurePipelinesTagInfo(ITFBuildProvider tfBuild)
    {
        // at the moment, there is no ability to know is it tag or not
        IsTag = tfBuild.Environment.Repository.Branch.StartsWith("refs/tags/");
        Name = IsTag
            ? tfBuild.Environment.Repository.Branch.Substring(10)
            : string.Empty;
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class AzurePipelinesRepositoryInfo : IRepositoryInfo
{
    public AzurePipelinesRepositoryInfo(ITFBuildProvider tfBuild)
    {
        Branch = tfBuild.Environment.Repository.Branch;
        Name = tfBuild.Environment.Repository.RepoName;
        Tag = new AzurePipelinesTagInfo(tfBuild);
    }

    public string Branch { get; }

    public string Name { get; }

    public ITagInfo Tag { get; }
}

public class AzurePipelinesPullRequestInfo : IPullRequestInfo
{
    public AzurePipelinesPullRequestInfo(ITFBuildProvider tfBuild)
    {
        //todo: update to `tfBuild.Environment.PullRequest.IsPullRequest` after upgrade to 0.33.0
        IsPullRequest = false;
    }

    public bool IsPullRequest { get; }
}

public class AzurePipelinesBuildInfo : IBuildInfo
{
    public AzurePipelinesBuildInfo(ITFBuildProvider tfBuild)
    {
        Number = tfBuild.Environment.Build.Number;
    }

    public string Number { get; }
}

public class AzurePipelinesBuildProvider : IBuildProvider
{
    public AzurePipelinesBuildProvider(ITFBuildProvider tfBuild)
    {
        Build = new AzurePipelinesBuildInfo(tfBuild);
        PullRequest = new AzurePipelinesPullRequestInfo(tfBuild);
        Repository = new AzurePipelinesRepositoryInfo(tfBuild);
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }
}