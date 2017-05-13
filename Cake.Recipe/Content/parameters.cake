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
    public static bool IsDevelopBranch { get; private set; }
    public static bool IsReleaseBranch { get; private set; }
    public static bool IsHotFixBranch { get; private set ; }
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
    public static WyamCredentials Wyam { get; private set; }
    public static BuildVersion Version { get; private set; }
    public static BuildPaths Paths { get; private set; }
    public static BuildTasks Tasks { get; set; }
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
    public static bool ShouldNotifyBetaReleases { get; private set; }

    public static FilePath MilestoneReleaseNotesFilePath { get; private set; }
    public static FilePath FullReleaseNotesFilePath { get; private set; }

    public static bool ShouldPublishMyGet { get; private set; }
    public static bool ShouldPublishChocolatey { get; private set; }
    public static bool ShouldPublishNuGet { get; private set; }
    public static bool ShouldPublishGitHub { get; private set; }
    public static bool ShouldGenerateDocumentation { get; private set; }
    public static bool ShouldExecuteGitLink { get; private set; }

    public static DirectoryPath WyamRootDirectoryPath { get; private set; }
    public static DirectoryPath WyamPublishDirectoryPath { get; private set; }
    public static FilePath WyamConfigurationFile { get; private set; }
    public static string WyamRecipe { get; private set; }
    public static string WyamTheme { get; private set; }
    public static string WyamSourceFiles { get; private set; }
    public static string WebHost { get; private set; }
    public static string WebLinkRoot { get; private set; }
    public static string WebBaseEditUrl { get; private set; }

    static BuildParameters()
    {
        Tasks = new BuildTasks();
    }

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

    public static bool CanUseWyam
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Wyam.AccessToken) &&
                !string.IsNullOrEmpty(BuildParameters.Wyam.DeployRemote) &&
                !string.IsNullOrEmpty(BuildParameters.Wyam.DeployBranch);
        }
    }

    public static void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    public static void SetBuildPaths(BuildPaths paths)
    {
        Paths = paths;
    }

    public static void PrintParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        context.Information("Printing Build Parameters...");
        context.Information("IsLocalBuild: {0}", IsLocalBuild);
        context.Information("IsPullRequest: {0}", IsPullRequest);
        context.Information("IsMainRepository: {0}", IsMainRepository);
        context.Information("IsTagged: {0}", IsTagged);
        context.Information("IsMasterBranch: {0}", IsMasterBranch);
        context.Information("IsDevelopBranch: {0}", IsDevelopBranch);
        context.Information("IsReleaseBranch: {0}", IsReleaseBranch);
        context.Information("IsHotFixBranch: {0}", IsHotFixBranch);
        context.Information("ShouldPostToGitter: {0}", ShouldPostToGitter);
        context.Information("ShouldPostToSlack: {0}", ShouldPostToSlack);
        context.Information("ShouldPostToTwitter: {0}", ShouldPostToTwitter);
        context.Information("ShouldPostToMicrosoftTeams: {0}", ShouldPostToMicrosoftTeams);
        context.Information("ShouldDownloadFullReleaseNotes: {0}", ShouldDownloadFullReleaseNotes);
        context.Information("ShouldDownloadMilestoneReleaseNotes: {0}", ShouldDownloadMilestoneReleaseNotes);
        context.Information("ShouldNotifyBetaReleases: {0}", ShouldNotifyBetaReleases);
        context.Information("ShouldGenerateDocumentation: {0}", ShouldGenerateDocumentation);
        context.Information("ShouldExecuteGitLink: {0}", ShouldExecuteGitLink);
        context.Information("IsRunningOnUnix: {0}", IsRunningOnUnix);
        context.Information("IsRunningOnWindows: {0}", IsRunningOnWindows);
        context.Information("IsRunningOnAppVeyor: {0}", IsRunningOnAppVeyor);
        context.Information("RepositoryOwner: {0}", RepositoryOwner);
        context.Information("RepositoryName: {0}", RepositoryName);
        context.Information("WyamRootDirectoryPath: {0}", WyamRootDirectoryPath);
        context.Information("WyamPublishDirectoryPath: {0}", WyamPublishDirectoryPath);
        context.Information("WyamConfigurationFile: {0}", WyamConfigurationFile);
        context.Information("WyamRecipe: {0}", WyamRecipe);
        context.Information("WyamTheme: {0}", WyamTheme);
        context.Information("WyamSourceFiles: {0}", WyamSourceFiles);
        context.Information("Wyam Deploy Branch: {0}", Wyam.DeployBranch);
        context.Information("Wyam Deploy Remote: {0}", Wyam.DeployRemote);
        context.Information("WebHost: {0}", WebHost);
        context.Information("WebLinkRoot: {0}", WebLinkRoot);
        context.Information("WebBaseEditUrl: {0}", WebBaseEditUrl);
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
        bool shouldNotifyBetaReleases = false,
        FilePath milestoneReleaseNotesFilePath = null,
        FilePath fullReleaseNotesFilePath = null,
        bool shouldPublishMyGet = true,
        bool shouldPublishChocolatey = true,
        bool shouldPublishNuGet = true,
        bool shouldPublishGitHub = true,
        bool shouldGenerateDocumentation = true,
        bool shouldExecuteGitLink = true,
        DirectoryPath wyamRootDirectoryPath = null,
        DirectoryPath wyamPublishDirectoryPath = null,
        FilePath wyamConfigurationFile = null,
        string wyamRecipe = null,
        string wyamTheme = null,
        string wyamSourceFiles = null,
        string webHost = null,
        string webLinkRoot = null,
        string webBaseEditUrl = null)
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

        WyamRootDirectoryPath = wyamRootDirectoryPath ?? context.MakeAbsolute(context.Directory("docs"));
        WyamPublishDirectoryPath = wyamPublishDirectoryPath ?? context.MakeAbsolute(context.Directory("BuildArtifacts/temp/_PublishedDocumentation"));
        WyamConfigurationFile = wyamConfigurationFile ?? context.MakeAbsolute((FilePath)"config.wyam");
        WyamRecipe = wyamRecipe ?? "Docs";
        WyamTheme = wyamTheme ?? "Samson";
        WyamSourceFiles = wyamSourceFiles ?? "../../" + SourceDirectoryPath.FullPath + "/**/{!bin,!obj,!packages,!*.Tests,}/**/*.cs";
        WebHost = webHost ?? string.Format("{0}.github.io", repositoryOwner);
        WebLinkRoot = webLinkRoot ?? title;
        WebBaseEditUrl = webBaseEditUrl ?? string.Format("https://github.com/{0}/{1}/tree/develop/docs/input/", repositoryOwner, title);

        ShouldPostToGitter = shouldPostToGitter;
        ShouldPostToSlack = shouldPostToSlack;
        ShouldPostToTwitter = shouldPostToTwitter;
        ShouldPostToMicrosoftTeams = shouldPostToMicrosoftTeams;
        ShouldDownloadFullReleaseNotes = shouldDownloadFullReleaseNotes;
        ShouldDownloadMilestoneReleaseNotes = shouldDownloadMilestoneReleaseNotes;
        ShouldNotifyBetaReleases = shouldNotifyBetaReleases;

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
        IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.AppVeyor.Environment.Repository.Branch);
        IsReleaseBranch = buildSystem.AppVeyor.Environment.Repository.Branch.StartsWith("release", StringComparison.OrdinalIgnoreCase);
        IsHotFixBranch = buildSystem.AppVeyor.Environment.Repository.Branch.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase);
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
        Wyam = GetWyamCredentials(context);
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
                                shouldPublishMyGet);

        ShouldPublishNuGet = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (IsMasterBranch || IsReleaseBranch || IsHotFixBranch) &&
                                IsTagged &&
                                shouldPublishNuGet);

        ShouldPublishChocolatey = (!IsLocalBuild &&
                                    !IsPullRequest &&
                                    IsMainRepository &&
                                    (IsMasterBranch || IsReleaseBranch || IsHotFixBranch) &&
                                    IsTagged &&
                                    shouldPublishChocolatey);

        ShouldPublishGitHub = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (IsMasterBranch || IsReleaseBranch || IsHotFixBranch) &&
                                IsTagged &&
                                shouldPublishGitHub);

        ShouldGenerateDocumentation = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (IsMasterBranch || IsDevelopBranch) &&
                                shouldGenerateDocumentation);

        ShouldExecuteGitLink = (!IsLocalBuild &&
                            !IsPullRequest &&
                            IsMainRepository &&
                            (IsMasterBranch || IsDevelopBranch || IsReleaseBranch || IsHotFixBranch) &&
                            shouldExecuteGitLink);
    }
}