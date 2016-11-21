public static class BuildParameters
{
    public static string Target { get; private set; }
    public static string Configuration { get; private set; }
    public static bool IsLocalBuild { get; private set; }
    public static bool IsRunningOnUnix { get; private set; }
    public static bool IsRunningOnWindows { get; private set; }
    public static bool IsRunningOnAppVeyor { get; private set; }
    public static bool IsPullRequest { get; private set; }
    public static bool IsMainRepository { get; private set; }
    public static bool IsMasterBranch { get; private set; }
    public static bool IsTagged { get; private set; }
    public static bool IsPublishBuild { get; private set; }
    public static bool IsReleaseBuild { get; private set; }
    public static GitHubCredentials GitHub { get; private set; }
    public static MicrosoftTeamsCredentials MicrosoftTeams { get; private set; }
    public static GitterCredentials Gitter { get; private set; }
    public static SlackCredentials Slack { get; private set; }
    public static TwitterCredentials Twitter { get; private set; }
    public static MyGetCredentials MyGet { get; private set; }
    public static NuGetCredentials NuGet { get; private set; }
    public static ChocolateyCredentials Chocolatey { get; private set; }
    public static AppVeyorCredentials AppVeyor { get; private set; }
    public static CoverallsCredentials Coveralls { get; private set; }
    public static BuildVersion Version { get; private set; }
    public static BuildPaths Paths { get; private set; }
    public static bool ShouldPostToGitter { get; private set; }
    public static bool ShouldPostToSlack { get; private set; }
    public static bool ShouldPostToTwitter { get; private set; }
    public static bool ShouldPostToMicrosoftTeams { get; private set; }

    public static bool CanPostToGitter
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Gitter.Token) &&
                !string.IsNullOrEmpty(BuildParameters.Gitter.RoomId);
        }
    }

    public static bool CanPostToSlack
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Slack.Token) &&
                !string.IsNullOrEmpty(BuildParameters.Slack.Channel);
        }
    }

    public static bool CanPostToTwitter
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerKey) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerSecret) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.AccessToken) &&
                !string.IsNullOrEmpty(BuildParameters.Twitter.AccessTokenSecret);
        }
    }

    public static bool CanPostToMicrosoftTeams
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.MicrosoftTeams.WebHookUrl);
        }
    }

    public static void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    public static void SetBuildPaths(BuildPaths paths)
    {
        Paths  = paths;
    }

    public static void SetParameters(
        ICakeContext context,
        BuildSystem buildSystem,
        string repositoryOwner,
        string repositoryName,
        bool shouldPostToGitter = true,
        bool shouldPostToSlack = true,
        bool shouldPostToTwitter = true,
        bool shouldPostToMicrosoftTeams = false)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        ShouldPostToGitter = shouldPostToGitter;
        ShouldPostToSlack = shouldPostToSlack;
        ShouldPostToTwitter = shouldPostToTwitter;
        ShouldPostToMicrosoftTeams = shouldPostToMicrosoftTeams;

        Target = context.Argument("target", "Default");
        Configuration = context.Argument("configuration", "Release");
        IsLocalBuild = buildSystem.IsLocalBuild;
        IsRunningOnUnix = context.IsRunningOnUnix();
        IsRunningOnWindows = context.IsRunningOnWindows();
        IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor;
        IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
        IsMainRepository = StringComparer.OrdinalIgnoreCase.Equals(string.Concat(repositoryOwner, "/", repositoryName), buildSystem.AppVeyor.Environment.Repository.Name);
        IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch);
        IsTagged = (
            buildSystem.AppVeyor.Environment.Repository.Tag.IsTag &&
            !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name)
        );
        GitHub = GetGitHubCredentials(context);
        MicrosoftTeams = GetMicrosoftTeamsCredentials(context);
        Gitter = GetGitterCredentials(context);
        Slack = GetSlackCredentials(context);
        Twitter = GetTwitterCredentials(context);
        MyGet = GetMyGetCredentials(context);
        NuGet = GetNuGetCredentials(context);
        Chocolatey = GetChocolateyCredentials(context);
        AppVeyor = GetAppVeyorCredentials(context);
        Coveralls = GetCoverallsCredentials(context);
        IsPublishBuild = new [] {
            "Create-Release-Notes"
        }.Any(
            releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, Target)
        );
        IsReleaseBuild = new [] {
            "Publish-NuGet-Packages",
            "Publish-Chocolatey-Packages",
            "Publish-GitHub-Release"
        }.Any(
            publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, Target)
        );
    }
}