///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class AzurePipelinesTagInfo : ITagInfo
{
    public AzurePipelinesTagInfo(IAzurePipelinesProvider azurePipelines)
    {
        const string refTags = "refs/tags/";
        // at the moment, there is no ability to know is it tag or not
        IsTag = azurePipelines.Environment.Repository.SourceBranchName.StartsWith(refTags);
        Name = IsTag
            ? azurePipelines.Environment.Repository.SourceBranchName.Substring(refTags.Length)
            : string.Empty;
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class AzurePipelinesRepositoryInfo : IRepositoryInfo
{
    public AzurePipelinesRepositoryInfo(IAzurePipelinesProvider azurePipelines)
    {
        Branch = azurePipelines.Environment.Repository.SourceBranchName;
        Name = azurePipelines.Environment.Repository.RepoName;
        Tag = new AzurePipelinesTagInfo(azurePipelines);
    }

    public string Branch { get; }

    public string Name { get; }

    public ITagInfo Tag { get; }
}

public class AzurePipelinesPullRequestInfo : IPullRequestInfo
{
    public AzurePipelinesPullRequestInfo(IAzurePipelinesProvider azurePipelines, ICakeEnvironment environment)
    {
        IsPullRequest = azurePipelines.Environment.PullRequest.IsPullRequest;
    }

    public bool IsPullRequest { get; }
}

public class AzurePipelinesBuildInfo : IBuildInfo
{
    public AzurePipelinesBuildInfo(IAzurePipelinesProvider azurePipelines)
    {
        Number = azurePipelines.Environment.Build.Number;
    }

    public string Number { get; }
}

public class AzurePipelinesBuildProvider : IBuildProvider
{
    public AzurePipelinesBuildProvider(IAzurePipelinesProvider azurePipelines, ICakeEnvironment environment)
    {
        Build = new AzurePipelinesBuildInfo(azurePipelines);
        PullRequest = new AzurePipelinesPullRequestInfo(azurePipelines, environment);
        Repository = new AzurePipelinesRepositoryInfo(azurePipelines);

        _azurePipelines = azurePipelines;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    public bool SupportsTokenlessCodecov { get; } = true;

    public IEnumerable<string> PrintVariables { get; } = new[] {
        "BUILD_BUILDID",
        "BUILD_BUILDNUMBER",
        "BUILD_REPOSITORY_NAME",
        "BUILD_SOURCEBRANCHNAME",
        "BUILD_SOURCEVERSION",
        "SYSTEM_PULLREQUEST_PULLREQUESTNUMBER",
        "SYSTEM_PULLREQUEST_TARGETBRANCH",
        "SYSTEM_TEAMFOUNDATIONSERVERURI",
        "SYSTEM_TEAMPROJECT",
        "TF_BUILD",
    };

    private readonly IAzurePipelinesProvider _azurePipelines;

    public void UploadArtifact(FilePath file)
    {
        _azurePipelines.Commands.UploadArtifact("", file, "artifacts");    
    }
}