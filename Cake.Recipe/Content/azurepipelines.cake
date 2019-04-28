///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

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
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }
}