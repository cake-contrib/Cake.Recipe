///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class LocalBuildTagInfo : ITagInfo
{
    public LocalBuildTagInfo(bool isTag, string name)
    {
        IsTag = isTag;
        Name = name;
    }

    public LocalBuildTagInfo(ICakeContext context)
    {
        // Test to see if current commit is a tag...
        context.Information("Testing to see if current commit contains a tag...");
        IEnumerable<string> redirectedStandardOutput;
        IEnumerable<string> redirectedError;

        var exitCode = context.StartProcess(
            "git",
            new ProcessSettings {
                Arguments = "tag -l --points-at HEAD",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            out redirectedStandardOutput,
            out redirectedError
        );

        if (exitCode == 0)
        {
            var lines = redirectedStandardOutput.ToList();
            if (lines.Any())
            {
                IsTag = true;
                Name = lines.FirstOrDefault();
                context.Information("Tag name is {0}", Name);
            }
            else
            {
                context.Information("No tag is present.");
            }
        }
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class LocalBuildRepositoryInfo : IRepositoryInfo
{
    public LocalBuildRepositoryInfo(ICakeContext context)
    {
        try
        {
            context.Information("Testing to see if valid git repository...");
            var rootPath = BuildParameters.RootDirectoryPath;
            rootPath = context.GitFindRootFromPath(rootPath);

            var gitTool = context.Tools.Resolve("git");

            if (gitTool == null)
            {
                context.Warning("Unable to find git, setting default values for repository properties...");
                Branch = "unknown";
                Name = "Local";
                Tag = new LocalBuildTagInfo(false, "unknown");
            }
            else
            {
                context.Information("Getting current branch name...");
                IEnumerable<string> redirectedStandardOutput;
                IEnumerable<string> redirectedError;

                var exitCode = context.StartProcess(
                    "git",
                    new ProcessSettings {
                        Arguments = "branch --show-current",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    out redirectedStandardOutput,
                    out redirectedError
                );

                if (exitCode == 0)
                {
                    var lines = redirectedStandardOutput.ToList();
                    if (lines.Any())
                    {
                        Branch = lines.FirstOrDefault();
                        context.Information("Branch name is {0}", Branch);
                    }
                }

                Name = "Local";
                Tag = new LocalBuildTagInfo(context);
            }
        }
        catch (LibGit2Sharp.RepositoryNotFoundException)
        {
            context.Warning("Unable to locate git repository, setting default values for repository properties...");

            Branch = "unknown";
            Name = "Local";
            Tag = new LocalBuildTagInfo(false, "unknown");
        }
    }

    public string Branch { get; }

    public string Name { get; }

    public ITagInfo Tag { get; }
}

public class LocalBuildPullRequestInfo : IPullRequestInfo
{
    public LocalBuildPullRequestInfo()
    {
        IsPullRequest = false;
    }

    public bool IsPullRequest { get; }
}

public class LocalBuildBuildInfo : IBuildInfo
{
    public LocalBuildBuildInfo()
    {
        Number = "-1";
    }

    public string Number { get; }
}

public class LocalBuildBuildProvider : IBuildProvider
{
    public LocalBuildBuildProvider(ICakeContext context)
    {
        Build = new LocalBuildBuildInfo();
        PullRequest = new LocalBuildPullRequestInfo();
        Repository = new LocalBuildRepositoryInfo(context);

        _context = context;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    public IEnumerable<string> PrintVariables { get; }

    private readonly ICakeContext _context;

    public void UploadArtifact(FilePath file)
    {
        _context.Information("Unable to upload artifacts as running local build");
    }
}
