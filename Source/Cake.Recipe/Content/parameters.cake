public enum BranchType
{
    Unknown,
    HotFix,
    Release,
    Develop,
    Master
}

public static class BuildParameters
{
    private static string _gitterMessage;
    private static string _microsoftTeamsMessage;
    private static string _twitterMessage;
    private static bool _shouldUseDeterministicBuilds;

    public static string Target { get; private set; }
    public static string Configuration { get; private set; }
    public static Cake.Core.Configuration.ICakeConfiguration CakeConfiguration { get; private set; }
    public static bool IsLocalBuild { get; private set; }
    public static PlatformFamily BuildAgentOperatingSystem => Platform.OperatingSystem;
    public static BuildPlatform Platform { get; private set; }
    public static bool IsRunningOnAppVeyor { get; private set; }
    public static bool IsRunningOnTravisCI { get; private set; }
    public static bool IsPullRequest { get; private set; }
    public static bool IsMainRepository { get; private set; }
    public static bool IsPublicRepository {get; private set; }
    public static bool IsTagged { get; private set; }
    public static bool IsPublishBuild { get; private set; }
    public static bool IsReleaseBuild { get; private set; }
    public static BranchType BranchType { get; private set; }
    public static bool IsDotNetCoreBuild { get; set; }
    public static bool IsNuGetBuild { get; set; }
    public static bool TransifexEnabled { get; set; }
    public static bool PrepareLocalRelease { get; set; }
    public static bool TreatWarningsAsErrors { get; set; }
    public static bool PublishReleasePackagesWasSuccessful { get; set; }
    public static string MasterBranchName { get; private set; }
    public static string DevelopBranchName { get; private set; }
    public static string EmailRecipient { get; private set; }
    public static string EmailSenderName { get; private set; }
    public static string EmailSenderAddress { get; private set; }
    public static bool ForceContinuousIntegration { get; private set; }
    public static PlatformFamily PreferredBuildAgentOperatingSystem { get; private set;}
    public static BuildProviderType PreferredBuildProviderType { get; private set; }

    public static List<PackageSourceData> PackageSources { get; private set; }

        public static bool IsRunningIntegrationTests
    {
        get
        {
            return string.Equals(BuildParameters.Target, "Run-Integration-Tests", StringComparison.OrdinalIgnoreCase);
        }
    }


    public static string StandardMessage
    {
        get { return "Version {0} of the {1} Addin has just been released, this will be available here https://www.nuget.org/packages/{1}, once package indexing is complete."; }
    }

    public static string GitterMessage
    {
        get { return _gitterMessage ?? "@/all " + StandardMessage; }
        set { _gitterMessage = value; }
    }

    public static string MicrosoftTeamsMessage
    {
        get { return _microsoftTeamsMessage ?? StandardMessage; }
        set { _microsoftTeamsMessage = value; }
    }

    public static string TwitterMessage
    {
        get { return _twitterMessage ?? StandardMessage; }
        set { _twitterMessage = value; }
    }

    public static GitHubCredentials GitHub { get; private set; }
    public static MicrosoftTeamsCredentials MicrosoftTeams { get; private set; }
    public static EmailCredentials Email { get; private set; }
    public static GitterCredentials Gitter { get; private set; }
    public static SlackCredentials Slack { get; private set; }
    public static TwitterCredentials Twitter { get; private set; }
    public static AppVeyorCredentials AppVeyor { get; private set; }
    public static CodecovCredentials Codecov { get; private set; }
    public static CoverallsCredentials Coveralls { get; private set; }
    public static TransifexCredentials Transifex { get; private set; }
    public static WyamCredentials Wyam { get; private set; }
    public static BuildPaths Paths { get; private set; }
    public static BuildTasks Tasks { get; set; }
    public static DirectoryPath RootDirectoryPath { get; private set; }
    public static FilePath SolutionFilePath { get; private set; }
    public static DirectoryPath SourceDirectoryPath { get; private set; }
    public static DirectoryPath SolutionDirectoryPath { get; private set; }
    public static DirectoryPath TestDirectoryPath { get; private set; }
    public static FilePath IntegrationTestScriptPath { get; private set; }
    public static string TestFilePattern { get; private set; }
    public static string Title { get; private set; }
    public static string ResharperSettingsFileName { get; private set; }
    public static string RepositoryOwner { get; private set; }
    public static string RepositoryName { get; private set; }
    public static string AppVeyorAccountName { get; private set; }
    public static string AppVeyorProjectSlug { get; private set; }

    public static TransifexMode TransifexPullMode { get; private set; }
    public static int TransifexPullPercentage { get; private set; }

    public static bool ShouldBuildNugetSourcePackage { get; private set; }
    public static bool ShouldPostToGitter { get; private set; }
    public static bool ShouldPostToSlack { get; private set; }
    public static bool ShouldPostToTwitter { get; private set; }
    public static bool ShouldPostToMicrosoftTeams { get; private set; }
    public static bool ShouldSendEmail { get; private set; }
    public static bool ShouldDownloadMilestoneReleaseNotes { get; private set;}
    public static bool ShouldDownloadFullReleaseNotes { get; private set;}
    public static bool ShouldNotifyBetaReleases { get; private set; }
    public static bool ShouldDeleteCachedFiles { get; private set; }

    public static FilePath MilestoneReleaseNotesFilePath { get; private set; }
    public static FilePath FullReleaseNotesFilePath { get; private set; }

    public static bool ShouldRunDupFinder { get; private set; }
    public static bool ShouldRunInspectCode { get; private set; }
    public static bool ShouldRunCoveralls { get; private set; }
    public static bool ShouldRunCodecov { get; private set; }
    public static bool ShouldRunDotNetCorePack { get; private set; }
    public static bool ShouldRunChocolatey { get; private set; }
    public static bool ShouldPublishGitHub { get; private set; }
    public static bool ShouldGenerateDocumentation { get; private set; }
    public static bool ShouldDocumentSourceFiles { get; private set; }
    public static bool ShouldRunIntegrationTests { get; private set; }
    public static bool ShouldCalculateVersion { get; private set; }
    public static bool ShouldUseTargetFrameworkPath { get; private set; }
    public static bool ShouldUseDeterministicBuilds
    {
        get
        {
            return !IsLocalBuild && _shouldUseDeterministicBuilds;
        }
    }

    public static DirectoryPath WyamRootDirectoryPath { get; private set; }
    public static DirectoryPath WyamPublishDirectoryPath { get; private set; }
    public static FilePath WyamConfigurationFile { get; private set; }
    public static string WyamRecipe { get; private set; }
    public static string WyamTheme { get; private set; }
    public static string WyamSourceFiles { get; private set; }
    public static string WebHost { get; private set; }
    public static string WebLinkRoot { get; private set; }
    public static string WebBaseEditUrl { get; private set; }

    public static FilePath NuSpecFilePath { get; private set; }

    public static FilePath NugetConfig { get; private set; }
    public static ICollection<string> NuGetSources { get; private set; }
    public static DirectoryPath RestorePackagesDirectory { get; private set; }

    public static IBuildProvider BuildProvider { get; private set; }

    static BuildParameters()
    {
        Tasks = new BuildTasks();
    }

    public static bool CanUseGitReleaseManager
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.GitHub.Token);
        }
    }

    public static bool CanSendEmail
    {
        get
        {
            return !string.IsNullOrEmpty(BuildParameters.Email.SmtpHost);
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

    public static bool CanPublishToCodecov
    {
        get
        {
            return ShouldRunCodecov && (
                BuildProvider.SupportsTokenlessCodecov ||
                !string.IsNullOrEmpty(Codecov.RepoToken)
            );
        }
    }

    public static bool CanPublishToCoveralls
    {
        get
        {
            return ShouldRunCoveralls && !string.IsNullOrEmpty(BuildParameters.Coveralls.RepoToken);
        }
    }

    public static bool CanPullTranslations
    {
        get
        {
            return BuildParameters.TransifexEnabled && !BuildParameters.IsPullRequest
                && (BuildParameters.IsRunningOnAppVeyor
                    || string.Equals(BuildParameters.Target, "Transifex-Pull-Translations", StringComparison.OrdinalIgnoreCase));
        }
    }

    public static bool CanPushTranslations
    {
        get
        {
            return BuildParameters.TransifexEnabled && !BuildParameters.IsPullRequest
                && (BuildParameters.IsRunningOnAppVeyor
                    || string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase));
        }
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
        context.Information("IsPublicRepository: {0}", IsPublicRepository);
        context.Information("IsTagged: {0}", IsTagged);
        context.Information("BranchType: {0}", BranchType);
        context.Information("TreatWarningsAsErrors: {0}", TreatWarningsAsErrors);
        context.Information("ShouldSendEmail: {0}", ShouldSendEmail);
        context.Information("ShouldPostToGitter: {0}", ShouldPostToGitter);
        context.Information("ShouldPostToSlack: {0}", ShouldPostToSlack);
        context.Information("ShouldPostToTwitter: {0}", ShouldPostToTwitter);
        context.Information("ShouldPostToMicrosoftTeams: {0}", ShouldPostToMicrosoftTeams);
        context.Information("ShouldDownloadFullReleaseNotes: {0}", ShouldDownloadFullReleaseNotes);
        context.Information("ShouldDownloadMilestoneReleaseNotes: {0}", ShouldDownloadMilestoneReleaseNotes);
        context.Information("ShouldNotifyBetaReleases: {0}", ShouldNotifyBetaReleases);
        context.Information("ShouldDeleteCachedFiles: {0}", ShouldDeleteCachedFiles);
        context.Information("ShouldGenerateDocumentation: {0}", ShouldGenerateDocumentation);
        context.Information("ShouldDocumentSourceFiles: {0}", ShouldDocumentSourceFiles);
        context.Information("ShouldRunIntegrationTests: {0}", ShouldRunIntegrationTests);
        context.Information("ShouldCalculateVersion: {0}", ShouldCalculateVersion);
        context.Information("BuildAgentOperatingSystem: {0}", BuildAgentOperatingSystem);
        context.Information("IsRunningOnAppVeyor: {0}", IsRunningOnAppVeyor);
        context.Information("RepositoryOwner: {0}", RepositoryOwner);
        context.Information("RepositoryName: {0}", RepositoryName);
        context.Information("TransifexEnabled: {0}", TransifexEnabled);
        context.Information("CanPullTranslations: {0}", CanPullTranslations);
        context.Information("CanPushTranslations: {0}", CanPushTranslations);
        context.Information("PrepareLocalRelease: {0}", PrepareLocalRelease);
        context.Information("BuildAgentOperatingSystem: {0}", BuildAgentOperatingSystem);
        context.Information("ForceContinuousIntegration: {0}", ForceContinuousIntegration);
        context.Information("PreferredBuildAgentOperatingSystem: {0}", PreferredBuildAgentOperatingSystem);
        context.Information("PreferredBuildProviderType: {0}", PreferredBuildProviderType);

        if (TransifexEnabled)
        {
            context.Information("TransifexPullMode: {0}", TransifexPullMode);
            context.Information("TransifexPullPercentage: {0}", TransifexPullPercentage);
        }

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
        context.Information("NuSpecFilePath: {0}", NuSpecFilePath);
        context.Information("NugetConfig: {0} ({1})", NugetConfig, context.FileExists(NugetConfig));
        context.Information("NuGetSources: {0}", string.Join(", ", NuGetSources));
        context.Information("RestorePackagesDirectory: {0}", RestorePackagesDirectory);
        context.Information("EmailRecipient: {0}", EmailRecipient);
        context.Information("EmailSenderName: {0}", EmailSenderName);
        context.Information("EmailSenderAddress: {0}", EmailSenderAddress);
    }

    public static void SetParameters(
        ICakeContext context,
        BuildSystem buildSystem,
        DirectoryPath sourceDirectoryPath,
        string title,
        FilePath solutionFilePath = null,
        DirectoryPath solutionDirectoryPath = null,
        DirectoryPath rootDirectoryPath = null,
        DirectoryPath testDirectoryPath = null,
        string testFilePattern = null,
        string integrationTestScriptPath = null,
        string resharperSettingsFileName = null,
        string repositoryOwner = null,
        string repositoryName = null,
        string appVeyorAccountName = null,
        string appVeyorProjectSlug = null,
        bool shouldPostToGitter = true,
        bool shouldPostToSlack = true,
        bool shouldPostToTwitter = true,
        bool shouldPostToMicrosoftTeams = false,
        bool shouldSendEmail = true,
        bool shouldDownloadMilestoneReleaseNotes = false,
        bool shouldDownloadFullReleaseNotes = false,
        bool shouldNotifyBetaReleases = false,
        bool shouldDeleteCachedFiles = false,
        bool shouldUseDeterministicBuilds = true,
        FilePath milestoneReleaseNotesFilePath = null,
        FilePath fullReleaseNotesFilePath = null,
        bool shouldRunChocolatey = true,
        bool shouldPublishGitHub = true,
        bool shouldGenerateDocumentation = true,
        bool shouldDocumentSourceFiles = true,
        bool shouldRunDupFinder = true,
        bool shouldRunInspectCode = true,
        bool shouldRunCoveralls = true,
        bool shouldRunCodecov = true,
        bool shouldRunDotNetCorePack = false,
        bool shouldBuildNugetSourcePackage = false,
        bool shouldRunIntegrationTests = false,
        bool shouldCalculateVersion = true,
        bool? shouldUseTargetFrameworkPath = null,
        bool? transifexEnabled = null,
        TransifexMode transifexPullMode = TransifexMode.OnlyTranslated,
        int transifexPullPercentage = 60,
        string gitterMessage = null,
        string microsoftTeamsMessage = null,
        string twitterMessage = null,
        DirectoryPath wyamRootDirectoryPath = null,
        DirectoryPath wyamPublishDirectoryPath = null,
        FilePath wyamConfigurationFile = null,
        string wyamRecipe = null,
        string wyamTheme = null,
        string wyamSourceFiles = null,
        string webHost = null,
        string webLinkRoot = null,
        string webBaseEditUrl = null,
        FilePath nuspecFilePath = null,
        bool isPublicRepository = true,
        FilePath nugetConfig = null,
        ICollection<string> nuGetSources = null,
        bool treatWarningsAsErrors = true,
        string masterBranchName = "master",
        string developBranchName = "develop",
        string emailRecipient = null,
        string emailSenderName = null,
        string emailSenderAddress = null,
        DirectoryPath restorePackagesDirectory = null,
        List<PackageSourceData> packageSourceDatas = null,
        PlatformFamily preferredBuildAgentOperatingSystem = PlatformFamily.Windows,
        BuildProviderType preferredBuildProviderType = BuildProviderType.AppVeyor
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        PreferredBuildAgentOperatingSystem = preferredBuildAgentOperatingSystem;
        PreferredBuildProviderType = preferredBuildProviderType;

        Platform = BuildPlatform.Create(context);

        Platform.CopyLibGit2Binaries("**/netstandard*/**/libgit2*.so", "**/linux-x64/libgit2*.so");
        Platform.PatchGitLib2ConfigFiles();

        BuildProvider = GetBuildProvider(context, buildSystem);

        EmailRecipient = emailRecipient;
        EmailSenderName = emailSenderName;
        EmailSenderAddress = emailSenderAddress;

        SourceDirectoryPath = sourceDirectoryPath;
        Title = title;
        SolutionFilePath = solutionFilePath ?? SourceDirectoryPath.CombineWithFilePath(Title + ".sln");
        SolutionDirectoryPath = solutionDirectoryPath ?? SourceDirectoryPath.Combine(Title);
        RootDirectoryPath = rootDirectoryPath ?? context.MakeAbsolute(context.Environment.WorkingDirectory);
        TestDirectoryPath = testDirectoryPath ?? sourceDirectoryPath;
        TestFilePattern = testFilePattern;
        IntegrationTestScriptPath = integrationTestScriptPath ?? context.MakeAbsolute((FilePath)"test.cake");
        ResharperSettingsFileName = resharperSettingsFileName ?? string.Format("{0}.sln.DotSettings", Title);
        RepositoryOwner = repositoryOwner ?? string.Empty;
        RepositoryName = repositoryName ?? Title;
        AppVeyorAccountName = appVeyorAccountName ?? RepositoryOwner.Replace("-", "").ToLower();
        AppVeyorProjectSlug = appVeyorProjectSlug ?? Title.Replace(".", "-").ToLower();

        TransifexEnabled = transifexEnabled ?? TransifexIsConfiguredForRepository(context);
        TransifexPullMode = transifexPullMode;
        TransifexPullPercentage = transifexPullPercentage;

        GitterMessage = gitterMessage;
        MicrosoftTeamsMessage = microsoftTeamsMessage;
        TwitterMessage = twitterMessage;

        WyamRootDirectoryPath = wyamRootDirectoryPath ?? context.MakeAbsolute(context.Directory("docs"));
        WyamPublishDirectoryPath = wyamPublishDirectoryPath ?? context.MakeAbsolute(context.Directory("BuildArtifacts/temp/_PublishedDocumentation"));
        WyamConfigurationFile = wyamConfigurationFile ?? context.MakeAbsolute((FilePath)"config.wyam");
        WyamRecipe = wyamRecipe ?? "Docs";
        WyamTheme = wyamTheme ?? "Samson";
        WyamSourceFiles = wyamSourceFiles ?? "../../" + SourceDirectoryPath.FullPath + "/**/{!bin,!obj,!packages,!*.Tests,}/**/*.cs";
        WebHost = webHost ?? string.Format("{0}.github.io", repositoryOwner);
        WebLinkRoot = webLinkRoot ?? RepositoryName;
        WebBaseEditUrl = webBaseEditUrl ?? string.Format("https://github.com/{0}/{1}/tree/{2}/docs/input/", repositoryOwner, title, developBranchName);

        ShouldPostToGitter = shouldPostToGitter;
        ShouldPostToSlack = shouldPostToSlack;
        ShouldPostToTwitter = shouldPostToTwitter;
        ShouldPostToMicrosoftTeams = shouldPostToMicrosoftTeams;
        ShouldSendEmail = shouldSendEmail;
        ShouldDownloadFullReleaseNotes = shouldDownloadFullReleaseNotes;
        ShouldDownloadMilestoneReleaseNotes = shouldDownloadMilestoneReleaseNotes;
        ShouldNotifyBetaReleases = shouldNotifyBetaReleases;
        ShouldDeleteCachedFiles = shouldDeleteCachedFiles;
        ShouldRunDupFinder = shouldRunDupFinder;
        ShouldRunInspectCode = shouldRunInspectCode;
        ShouldRunCoveralls = shouldRunCoveralls;
        ShouldRunCodecov = shouldRunCodecov;
        ShouldRunDotNetCorePack = shouldRunDotNetCorePack;
        ShouldBuildNugetSourcePackage = shouldBuildNugetSourcePackage;
        ShouldCalculateVersion = shouldCalculateVersion;
        _shouldUseDeterministicBuilds = shouldUseDeterministicBuilds;
        ShouldUseTargetFrameworkPath = shouldUseTargetFrameworkPath ?? !context.IsRunningOnWindows();

        MilestoneReleaseNotesFilePath = milestoneReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("CHANGELOG.md");
        FullReleaseNotesFilePath = fullReleaseNotesFilePath ?? RootDirectoryPath.CombineWithFilePath("ReleaseNotes.md");

        NuSpecFilePath = nuspecFilePath ?? context.MakeAbsolute((FilePath)"./Cake.Recipe/Cake.Recipe.nuspec");

        NugetConfig = context.MakeAbsolute(nugetConfig ?? (FilePath)"./NuGet.Config");
        NuGetSources = nuGetSources;

        if (nuGetSources == null)
        {
            if (context.FileExists(NugetConfig))
            {
                NuGetSources = (
                                    from configuration in System.Xml.Linq.XDocument.Load(NugetConfig.FullPath).Elements("configuration")
                                    from packageSources in configuration.Elements("packageSources")
                                    from add in packageSources.Elements("add")
                                    from value in add.Attributes("value")
                                    select value.Value
                                ).ToArray();
            }
            else
            {
                // TODO Use parameter for Cake Contrib feed from environment variable, similar to BuildParameters.MyGet.SourceUrl
                NuGetSources = new []{
                    "https://api.nuget.org/v3/index.json",
                    "https://www.myget.org/F/cake-contrib/api/v3/index.json"
                };
            }
        }

        RestorePackagesDirectory = restorePackagesDirectory;

        Target = context.Argument("target", "Default");
        Configuration = context.Argument("configuration", "Release");
        PrepareLocalRelease = context.Argument("prepareLocalRelease", false);
        ForceContinuousIntegration = context.Argument("forceContinuousIntegration", false);

        CakeConfiguration = context.GetConfiguration();
        MasterBranchName = masterBranchName;
        DevelopBranchName = developBranchName;
        // Workaround until bumping to cake 0.37.0+
        IsLocalBuild = buildSystem.IsLocalBuild && BuildProvider.GetType() != typeof(GitHubActionBuildProvider);
        IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor;
        IsRunningOnTravisCI = buildSystem.IsRunningOnTravisCI;
        IsPullRequest = BuildProvider.PullRequest.IsPullRequest;
        IsMainRepository = StringComparer.OrdinalIgnoreCase.Equals(string.Concat(repositoryOwner, "/", RepositoryName), BuildProvider.Repository.Name);
        IsPublicRepository = isPublicRepository;

        IsTagged = (
            BuildProvider.Repository.Tag.IsTag &&
            !string.IsNullOrWhiteSpace(BuildProvider.Repository.Tag.Name)
        );

        var branchName = BuildProvider.Repository.Branch ?? string.Empty; // This is just to prevent any null reference exceptions
        if (StringComparer.OrdinalIgnoreCase.Equals(masterBranchName, branchName))
        {
            BranchType = BranchType.Master;
        }
        else if (StringComparer.OrdinalIgnoreCase.Equals(developBranchName, branchName))
        {
            BranchType = BranchType.Develop;
        }
        else if (branchName.StartsWith("release", StringComparison.OrdinalIgnoreCase))
        {
            BranchType = BranchType.Release;
        }
        else if (branchName.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase))
        {
            BranchType = BranchType.HotFix;
        }
        else if (IsTagged)
        {
            BranchType = BranchType.Unknown;
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
                        Arguments = "branch --contains HEAD",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    },
                    out redirectedStandardOutput,
                    out redirectedError
                );

                if (exitCode == 0)
                {
                    var lines = redirectedStandardOutput.Select(c => c.TrimStart(new []{ ' ', '*' })).ToList();

                    if (lines.Contains(masterBranchName))
                    {
                        BranchType = BranchType.Master;
                    }
                    else if (lines.Contains(developBranchName))
                    {
                        BranchType = BranchType.Develop;
                    }
                    else if (lines.Any(l => l.StartsWith("release", StringComparison.OrdinalIgnoreCase)))
                    {
                        BranchType = BranchType.Release;
                    }
                    else if (lines.Any(l => l.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase)))
                    {
                        BranchType = BranchType.HotFix;
                    }
                }
            }
        }
        else
        {
            BranchType = BranchType.Unknown;
        }

        TreatWarningsAsErrors = treatWarningsAsErrors;

        GitHub = GetGitHubCredentials(context);
        MicrosoftTeams = GetMicrosoftTeamsCredentials(context);
        Email = GetEmailCredentials(context);
        Gitter = GetGitterCredentials(context);
        Slack = GetSlackCredentials(context);
        Twitter = GetTwitterCredentials(context);
        AppVeyor = GetAppVeyorCredentials(context);
        Codecov = GetCodecovCredentials(context);
        Coveralls = GetCoverallsCredentials(context);
        Transifex = GetTransifexCredentials(context);
        Wyam = GetWyamCredentials(context);
        IsPublishBuild = new [] {
            "Create-Release-Notes"
        }.Any(
            releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, Target)
        );
        IsReleaseBuild = new [] {
            "Publish-PreRelease-Packages",
            "Publish-Release-Packages",
            "Publish-GitHub-Release"
        }.Any(
            publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, Target)
        );

        SetBuildPaths(BuildPaths.GetPaths(context));

        ShouldRunChocolatey = shouldRunChocolatey;

        ShouldPublishGitHub = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (BuildParameters.BranchType == BranchType.Master || BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix) &&
                                IsTagged &&
                                BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem &&
                                BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type &&
                                shouldPublishGitHub);

        ShouldGenerateDocumentation = (!IsLocalBuild &&
                                !IsPullRequest &&
                                IsMainRepository &&
                                (BuildParameters.BranchType == BranchType.Master || BuildParameters.BranchType == BranchType.Develop) &&
                                BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem &&
                                BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type &&
                                shouldGenerateDocumentation);

        ShouldDocumentSourceFiles = shouldDocumentSourceFiles;

        ShouldRunIntegrationTests = (((!IsLocalBuild && !IsPullRequest && IsMainRepository) &&
                                        (BuildParameters.BranchType == BranchType.Master || BuildParameters.BranchType == BranchType.Develop || BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix) &&
                                        context.FileExists(context.MakeAbsolute(BuildParameters.IntegrationTestScriptPath))) ||
                                        shouldRunIntegrationTests);

        if (packageSourceDatas?.Any() ?? false)
        {
            PackageSources = packageSourceDatas;
        }
        else
        {
            PackageSources = new List<PackageSourceData>();

            // Try to get the deprecated way of doing things, set them as default sources
            var myGetUrl = context.EnvironmentVariable("MYGET_SOURCE");
            if (!string.IsNullOrEmpty(myGetUrl))
            {
                PackageSources.Add(new PackageSourceData(context, "MYGET", myGetUrl, FeedType.NuGet, false));
                PackageSources.Add(new PackageSourceData(context, "MYGET", myGetUrl, FeedType.Chocolatey, false));
            }

            var nuGetUrl = context.EnvironmentVariable("NUGET_SOURCE");
            if (!string.IsNullOrEmpty(nuGetUrl))
            {
                PackageSources.Add(new PackageSourceData(context, "NUGET", nuGetUrl));
            }

            var chocolateyUrl = context.EnvironmentVariable("CHOCOLATEY_SOURCE");
            if (!string.IsNullOrEmpty(chocolateyUrl))
            {
                PackageSources.Add(new PackageSourceData(context, "CHOCOLATEY", chocolateyUrl, FeedType.Chocolatey));
            }

            // The following aren't deprecated sources, but rather suggested defaults going forward, so check
            // for the environment variables being set, if they are, add them to the list
            var azureUrl = context.EnvironmentVariable("AZURE_SOURCE");
            if (!string.IsNullOrEmpty(azureUrl))
            {
                PackageSources.Add(new PackageSourceData(context, "AZURE", azureUrl, FeedType.NuGet, false));
            }

            var gprUrl = context.EnvironmentVariable("GPR_SOURCE");
            if(!string.IsNullOrEmpty(gprUrl))
            {
                PackageSources.Add(new PackageSourceData(context, "GPR", gprUrl, FeedType.NuGet, false));
            }
        }
    }
}
