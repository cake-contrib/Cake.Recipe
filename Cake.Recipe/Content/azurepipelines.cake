///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class AzurePipelinesTagInfo : ITagInfo
{
    public AzurePipelinesTagInfo(IAzurePipelinesProvider azurePipelines)
    {
        const string refTags = "refs/tags/";
        // at the moment, there is no ability to know is it tag or not
        IsTag = azurePipelines.Environment.Repository.SourceBranch.StartsWith(refTags);
        Name = IsTag
            ? azurePipelines.Environment.Repository.SourceBranch.Substring(refTags.Length)
            : string.Empty;
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class AzurePipelinesRepositoryInfo : IRepositoryInfo
{
    public AzurePipelinesRepositoryInfo(IAzurePipelinesProvider azurePipelines, ICakeContext context)
    {
        Name = azurePipelines.Environment.Repository.RepoName;
        
        // This trimming is not perfect, as it will remove part of a
        // branch name if the branch name itself contains a '/'
        var tempName = azurePipelines.Environment.Repository.SourceBranch;
        const string headPrefix = "refs/heads/";
        const string tagPrefix = "refs/tags/";

        if (!string.IsNullOrEmpty(tempName))
        {
            if (tempName.StartsWith(headPrefix))
            {
                tempName = tempName.Substring(headPrefix.Length);
            }
            else if (tempName.StartsWith(tagPrefix))
            {
                context.Information("Attempting to find branch name using git...");
                var gitTool = context.Tools.Resolve("git");
                if (gitTool == null)
                {
                    gitTool = context.Tools.Resolve("git.exe");
                }

                if (gitTool != null)
                {
                    IEnumerable<string> redirectedStandardOutput;
                    IEnumerable<string> redirectedError;

                    var exitCode = context.StartProcess(
                        gitTool,
                        new ProcessSettings {
                            Arguments = "branch -r --contains " + tempName,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                        },
                        out redirectedStandardOutput,
                        out redirectedError
                    );

                    if (exitCode == 0)
                    {
                        var lines = redirectedStandardOutput.ToList();
                        if (lines.Count == 1)
                        {
                            context.Information("There is one line");
                            context.Information("lines[0]: {0}", lines[0]);
                            tempName = lines[0].TrimStart(new []{ ' ', '*' }).Replace("origin/", string.Empty);
                        }
                        else if(lines.Count > 1)
                        {
                            
                            context.Information("There are more than one line");
                            foreach(var line in lines)
                            {
                                var trimmedLine = line.TrimStart(new []{ ' ', '*' }).Replace("origin/", string.Empty);
                                context.Information("trimmedLine: {0}", trimmedLine);
                                
                                // This is a crude check to make sure that we are not looking at a ref
                                // to a SHA of the current commit.  If it is, we don't want to return that
                                if (trimmedLine.Length != 40)
                                {
                                    context.Information("tempName is not 40");
                                    tempName = trimmedLine;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        context.Information("Something went wrong...");
                    }
                }
                else
                {
                    context.Information("Unable to find git :-(");
                }
            }
            else if (tempName.IndexOf('/') >= 0)
            {
                context.Information("Resorting to checking for last slash...");
                tempName = tempName.Substring(tempName.LastIndexOf('/') + 1);
            }
        }

        context.Information("tempName: {0}", tempName);
        Branch = tempName;

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
    public AzurePipelinesBuildProvider(IAzurePipelinesProvider azurePipelines, ICakeEnvironment environment, ICakeContext context)
    {
        Build = new AzurePipelinesBuildInfo(azurePipelines);
        PullRequest = new AzurePipelinesPullRequestInfo(azurePipelines, environment);
        Repository = new AzurePipelinesRepositoryInfo(azurePipelines, context);

        _azurePipelines = azurePipelines;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    public bool SupportsTokenlessCodecov { get; } = true;

    public BuildProviderType Type { get; } = BuildProviderType.AzurePipelines;

    public IEnumerable<string> PrintVariables { get; } = new[] {
        "BUILD_BUILDID",
        "BUILD_BUILDNUMBER",
        "BUILD_REPOSITORY_NAME",
        "BUILD_SOURCEBRANCH",
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