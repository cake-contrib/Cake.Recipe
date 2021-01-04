public class GitHubActionTagInfo : ITagInfo
{
    public GitHubActionTagInfo(ICakeContext context)
    {
        var tempName = context.BuildSystem().GitHubActions.Environment.Workflow.Ref;
        if (!string.IsNullOrEmpty(tempName) && tempName.IndexOf("tags/") >= 0)
        {
            IsTag = true;
            Name = tempName.Substring(tempName.LastIndexOf('/') + 1);
        }
    }

    public bool IsTag { get; }
    public string Name { get; }
}

public class GitHubActionRepositoryInfo : IRepositoryInfo
{
    public GitHubActionRepositoryInfo(ICakeContext context)
    {
        Name = context.BuildSystem().GitHubActions.Environment.Workflow.Repository;
        var baseRef = context.BuildSystem().GitHubActions.Environment.Workflow.BaseRef;
        if (!string.IsNullOrEmpty(baseRef))
        {
            Branch = baseRef;
        }
        else
        {
            // This trimming is not perfect, as it will remove part of a
            // branch name if the branch name itself contains a '/'
            var tempName = context.BuildSystem().GitHubActions.Environment.Workflow.Ref;
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
                    var gitTool = context.Tools.Resolve("git");

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
                            if (lines.Count != 0)
                            {
                                tempName = lines[0].TrimStart(new []{ ' ', '*' }).Replace("origin/", string.Empty);
                            }
                        }
                    }
                }
                else if (tempName.IndexOf('/') >= 0)
                {
                    tempName = tempName.Substring(tempName.LastIndexOf('/') + 1);
                }
            }

            Branch = tempName;
        }
        Tag = new GitHubActionTagInfo(context);
    }

    public string Branch { get; }
    public string Name { get; }
    public ITagInfo Tag { get; }
}

public class GitHubActionPullRequestInfo : IPullRequestInfo
{
    public GitHubActionPullRequestInfo(ICakeContext context)
    {
        var headRef = context.BuildSystem().GitHubActions.Environment.Workflow.HeadRef;
        var branchRef = context.BuildSystem().GitHubActions.Environment.Workflow.Ref;

        IsPullRequest = !string.IsNullOrEmpty(headRef) && !string.IsNullOrEmpty(branchRef)
            && branchRef.IndexOf("refs/pull/") >= 0 && branchRef.IndexOf("/merge") >= 0;
    }

    public bool IsPullRequest { get; }
}

public class GitHubActionBuildInfo : IBuildInfo
{
    public GitHubActionBuildInfo(ICakeContext context)
    {
        Number = context.BuildSystem().GitHubActions.Environment.Workflow.RunNumber.ToString();
    }

    public string Number { get; }
}

public class GitHubActionBuildProvider : IBuildProvider
{
    private readonly ICakeContext _context;

    public GitHubActionBuildProvider(ICakeContext context)
    {
        Build = new GitHubActionBuildInfo(context);
        PullRequest = new GitHubActionPullRequestInfo(context);
        Repository = new GitHubActionRepositoryInfo(context);

        _context = context;
    }

    public IBuildInfo Build { get; }
    public IPullRequestInfo PullRequest { get; }
    public IRepositoryInfo Repository { get; }

    public bool SupportsTokenlessCodecov { get; } = true;

    public BuildProviderType Type { get; } = BuildProviderType.GitHubActions;

    public IEnumerable<string> PrintVariables { get; } = new[] {
        "CI",
        "HOME",
        "GITHUB_WORKFLOW",
        "GITHUB_RUN_ID",
        "GITHUB_RUN_NUMBER",
        "GITHUB_ACTION",
        "GITHUB_ACTIONS",
        "GITHUB_ACTOR",
        "GITHUB_REPOSITORY",
        "GITHUB_EVENT_NAME",
        "GITHUB_EVENT_PATH",
        "GITHUB_WORKSPACE",
        "GITHUB_SHA",
        "GITHUB_REF",
        "GITHUB_HEAD_REF",
        "GITHUB_BASE_REF"
    };

    public void UploadArtifact(FilePath file)
    {
        _context.Information("Uploading artifacts is currently not supported in Cake.Recipe. Please use the actions/upload-artifacts GitHub Action");
    }
}
