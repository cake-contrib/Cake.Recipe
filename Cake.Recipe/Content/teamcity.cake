///////////////////////////////////////////////////////////////////////////////
// BUILD PROVIDER
///////////////////////////////////////////////////////////////////////////////

public class TeamCityTagInfo : ITagInfo
{
    public TeamCityTagInfo(ICakeContext context)
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
            if (redirectedStandardOutput.Count() != 0)
            {
                IsTag = true;
                Name = redirectedStandardOutput.FirstOrDefault();
            }
        }
    }

    public bool IsTag { get; }

    public string Name { get; }
}

public class TeamCityRepositoryInfo : IRepositoryInfo
{
    public TeamCityRepositoryInfo(ITeamCityProvider teamCity, ICakeContext context)
    {
        Branch = context.Environment.GetEnvironmentVariable("vcsroot.branch");
        Name = teamCity.Environment.Build.BuildConfName;
        Tag = new TeamCityTagInfo(context);
    }

    public string Branch { get; }

    public string Name { get; }

    public ITagInfo Tag { get; }
}

public class TeamCityPullRequestInfo : IPullRequestInfo
{
    public TeamCityPullRequestInfo(ITeamCityProvider teamCity)
    {
        IsPullRequest = teamCity.Environment.PullRequest.IsPullRequest;
    }

    public bool IsPullRequest { get; }
}

public class TeamCityBuildInfo : IBuildInfo
{
    public TeamCityBuildInfo(ITeamCityProvider teamCity)
    {
        Number = teamCity.Environment.Build.Number;
    }

    public string Number { get; }
}

public class TeamCityBuildProvider : IBuildProvider
{
    public TeamCityBuildProvider(ITeamCityProvider teamCity, ICakeContext context)
    {
        Build = new TeamCityBuildInfo(teamCity);
        PullRequest = new TeamCityPullRequestInfo(teamCity);
        Repository = new TeamCityRepositoryInfo(teamCity, context);

        _teamCity = teamCity;
    }

    public IRepositoryInfo Repository { get; }

    public IPullRequestInfo PullRequest { get; }

    public IBuildInfo Build { get; }

    private readonly ITeamCityProvider _teamCity;

    public void UploadArtifact(FilePath file)
    {
        _teamCity.PublishArtifacts(file.FullPath);
    }
}
