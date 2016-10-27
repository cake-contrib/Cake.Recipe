public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsPullRequest { get; private set; }
    public bool IsMainRepository { get; private set; }
    public bool IsMasterBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPublishBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public GitHubCredentials GitHub { get; private set; }
    public GitterCredentials Gitter { get; private set; }
    public SlackCredentials Slack { get; private set; }
    public TwitterCredentials Twitter { get; private set; }
    public MyGetCredentials MyGet { get; private set; }
    public NuGetCredentials NuGet { get; private set; }
    public ChocolateyCredentials Chocolatey { get; private set; }
    public AppVeyorCredentials AppVeyor { get; private set; }
    public CoverallsCredentials Coveralls { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }

    public void SetBuildVersion(BuildVersion version)
    {
        Version  = version;
    }

    public void SetBuildPaths(BuildPaths paths)
    {
        Paths  = paths;
    }

    public static BuildParameters GetParameters(
        ICakeContext context,
        BuildSystem buildSystem,
        string repositoryOwner,
        string repositoryName
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var configuration = context.Argument("configuration", "Release");
        return new BuildParameters {
            Target = target,
            Configuration = configuration,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
            IsMainRepository = StringComparer.OrdinalIgnoreCase.Equals(string.Concat(repositoryOwner, "/", repositoryName), buildSystem.AppVeyor.Environment.Repository.Name),
            IsMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch),
            IsTagged = (
                buildSystem.AppVeyor.Environment.Repository.Tag.IsTag &&
                !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name)
            ),
            GitHub = GetGitHubCredentials(context),
            Gitter = GetGitterCredentials(context),
            Slack = GetSlackCredentials(context),
            Twitter = GetTwitterCredentials(context),
            MyGet = GetMyGetCredentials(context),
            NuGet = GetNuGetCredentials(context),
            Chocolatey = GetChocolateyCredentials(context),
            AppVeyor = GetAppVeyorCredentials(context),
            Coveralls = GetCoverallsCredentials(context),
            IsPublishBuild = new [] {
                "Create-Release-Notes"
            }.Any(
                releaseTarget => StringComparer.OrdinalIgnoreCase.Equals(releaseTarget, target)
            ),
            IsReleaseBuild = new [] {
                "Publish-NuGet-Packages",
                "Publish-Chocolatey-Packages",
                "Publish-GitHub-Release"
            }.Any(
                publishTarget => StringComparer.OrdinalIgnoreCase.Equals(publishTarget, target)
            )
        };
    }
}