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
        var branchName = context.Environment.GetEnvironmentVariable("vcsroot.branch");
        if(string.IsNullOrEmpty(branchName)) 
        {
            throw new Exception(@"Environment variable ""vcsroot.branch"" could not be found. Cake.Recipe needs ""vcsroot.branch"" exposed as a build parameter.");
        }

        Branch = branchName.Replace("refs/heads/", string.Empty);
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

    public bool SupportsTokenlessCodecov { get; } = false;

    public BuildProviderType Type { get; } = BuildProviderType.TeamCity;

    public IEnumerable<string> PrintVariables { get; } = new[] {
        "TEAMCITY_BUILD_BRANCH",
        "TEAMCITY_BUILD_COMMIT",
        "TEAMCITY_BUILD_ID",
        "TEAMCITY_BUILD_REPOSITORY",
        "TEAMCITY_BUILD_URL",
        "TEAMCITY_VERSION",
        "vcsroot.branch",
    };

    private readonly ITeamCityProvider _teamCity;

    public void UploadArtifact(FilePath file)
    {
        _teamCity.PublishArtifacts(file.FullPath);
    }
}
