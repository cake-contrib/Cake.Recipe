///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class AzurePipelinesTagInfo : ITagInfo
{
    public AzurePipelinesTagInfo(ITFBuildProvider tfBuild)
    {
        const string refTags = "refs/tags/";
        // at the moment, there is no ability to know is it tag or not
        IsTag = tfBuild.Environment.Repository.Branch.StartsWith(refTags);
        Name = IsTag
            ? tfBuild.Environment.Repository.Branch.Substring(refTags.Length)
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
    public AzurePipelinesPullRequestInfo(ITFBuildProvider tfBuild, ICakeEnvironment environment)
    {
        IsPullRequest = tfBuild.Environment.PullRequest.IsPullRequest;
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
    public AzurePipelinesBuildProvider(ITFBuildProvider tfBuild, ICakeEnvironment environment)
    {
        Build = new AzurePipelinesBuildInfo(tfBuild);
        PullRequest = new AzurePipelinesPullRequestInfo(tfBuild, environment);
        Repository = new AzurePipelinesRepositoryInfo(tfBuild);

        _tfBuild = tfBuild;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    private readonly ITFBuildProvider _tfBuild;

    public void UploadArtifact(FilePath file)
    {
        _tfBuild.Commands.UploadArtifact("", file, "artifacts");    
    }
}