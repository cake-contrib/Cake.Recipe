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
    
    public static DirectoryPath RootDirectoryPath { get; private set; }
    public static FilePath SolutionFilePath { get; private set; }
    public static DirectoryPath SourceDirectoryPath { get; private set; }
    public static DirectoryPath SolutionDirectoryPath { get; private set; }
    public static string Title { get; private set; }
    public static string ResharperSettingsFileName { get; private set; }
    public static string RepositoryOwner { get; private set; }
    public static string RepositoryName { get; private set; }
    public static string AppVeyorAccountName { get; private set; }
    public static string AppVeyorProjectSlug { get; private set; }
    
    public static bool ShouldPostToGitter { get; private set; }
    public static bool ShouldPostToSlack { get; private set; }
    public static bool ShouldPostToTwitter { get; private set; }
    public static bool ShouldPostToMicrosoftTeams { get; private set; }
    public static bool ShouldDownloadMilestoneReleaseNotes { get; private set;}
    public static bool ShouldDownloadFullReleaseNotes { get; private set;}

    public static FilePath MilestoneReleaseNotesFilePath { get; private set; }
    public static FilePath FullReleaseNotesFilePath { get; private set; }

    public static bool ShouldPublishMyGet { get; private set;}
    public static bool ShouldPublishChocolatey { get; private set;}
    public static bool ShouldPublishNuGet { get; private set;}
    public static bool ShouldPublishGitHub { get; private set;}

    public static bool CanUseGitReleaseManager
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.GitHub.UserName) &&
                !string.IsNullOrEmpty(BuildParameters.GitHub.Password);
        }
    }

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
        DirectoryPath sourceDirectoryPath,
        string title,
        FilePath solutionFilePath = null,
        DirectoryPath solutionDirectoryPath = null,
        DirectoryPath rootDirectoryPath = null,
        string resharperSettingsFileName = null,
        string repositoryOwner = null,
        string repositoryName = null,
        string appVeyorAccountName = null,
        string appVeyorProjectSlug = null,
        bool shouldPostToGitter = true,
        bool shouldPostToSlack = true,
        bool shouldPostToTwitter = true,
        bool shouldPostToMicrosoftTeams = false,
        bool shouldDownloadMilestoneReleaseNotes = false,
        bool shouldDownloadFullReleaseNotes = false,
        FilePath milestoneReleaseNotesFilePath = null,
        FilePath fullReleaseNotesFilePath = null,
        bool shouldPublishMyGet = true,
        bool shouldPublishChocolatey = true,
        bool shouldPublishNuGet = true,
        bool shouldPublishGitHub = true)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }
        
        SourceDirectoryPath = sourceDirectoryPath;
        Title = title;
        SolutionFilePath = solutionFilePath ?? SourceDirectoryPath.CombineWithFilePath(Title + ".sln");
        SolutionDirectoryPath = solutionDirectoryPath ?? SourceDirectoryPath.Combine(Title);
        RootDirectoryPath = rootDirectoryPath ?? context.MakeAbsolute(context.Environment.WorkingDirectory);
        ResharperSettingsFileName = resharperSettingsFileName ?? string.Format("{0}.sln.DotSettings", Title);
        RepositoryOwner = repositoryOwner ?? string.Empty;
        RepositoryName = repositoryName ?? Title;
        AppVeyorAccountName = appVeyorAccountName ?? RepositoryOwner.Replace("-", "").ToLower();
        AppVeyorProjectSlug = appVeyorProjectSlug ?? Title.Replace(".", "-").ToLower();

        ShouldPostToGitter = shouldPostToGitter;
        ShouldPostToSlack = shouldPostToSlack;
        ShouldPostToTwitter = shouldPostToTwitter;
        ShouldPostToMicrosoftTeams = shouldPostToMicrosoftTeams;
        ShouldDownloadFullReleaseNotes = shouldDownloadFullReleaseNotes;
        ShouldDownloadMilestoneReleaseNotes = shouldDownloadMilestoneReleaseNotes;

        MilestoneReleaseNotesFilePath = milestoneReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("CHANGELOG.md");
        FullReleaseNotesFilePath = fullReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("ReleaseNotes.md");

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

        SetBuildPaths(BuildPaths.GetPaths(context));

        ShouldPublishMyGet = (!IsLocalBuild &&
                        !IsPullRequest &&
                        IsMainRepository &&
                        (IsTagged || !IsMasterBranch) &&
                        DirectoryExists(Paths.Directories.NuGetPackages) || DirectoryExists(Paths.Directories.ChocolateyPackages) &&
                        shouldPublishMyGet);

        ShouldPublishNuGet = (!IsLocalBuild &&
                              !IsPullRequest &&
                              IsMainRepository &&
                              IsMasterBranch &&
                              IsTagged &&
                              DirectoryExists(Paths.Directories.NuGetPackages) &&
                              shouldPublishNuGet);
        
        ShouldPublishChocolatey = (!IsLocalBuild &&
                                  !IsPullRequest &&
                                  IsMainRepository &&
                                  IsMasterBranch &&
                                  IsTagged &&
                                  DirectoryExists(Paths.Directories.ChocolateyPackages) &&
                                  shouldPublishChocolatey);

        ShouldPublishGitHub = (!IsLocalBuild &&
                               !IsPullRequest &&
                               IsMainRepository &&
                               IsMasterBranch &&
                               IsTagged &&
                               shouldPublishGitHub);
    }
}