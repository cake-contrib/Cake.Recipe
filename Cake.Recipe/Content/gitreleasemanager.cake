///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Create-Release-Notes")
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerCreate(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, new GitReleaseManagerCreateSettings {
                Milestone         = BuildParameters.Version.Milestone,
                Name              = BuildParameters.Version.Milestone,
                Prerelease        = true,
                TargetCommitish   = "master"
            });
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

Task("Export-Release-Notes")
    .WithCriteria(() => BuildParameters.ShouldDownloadMilestoneReleaseNotes || BuildParameters.ShouldDownloadFullReleaseNotes)
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .WithCriteria(() => BuildParameters.IsMasterBranch || BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch)
    .WithCriteria(() => BuildParameters.IsTagged)
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            if(BuildParameters.ShouldDownloadMilestoneReleaseNotes)
            {
                GitReleaseManagerExport(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.MilestoneReleaseNotesFilePath, new GitReleaseManagerExportSettings {
                    TagName         = BuildParameters.Version.Milestone
                });
            }

            if(BuildParameters.ShouldDownloadFullReleaseNotes)
            {
                GitReleaseManagerExport(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.FullReleaseNotesFilePath);
            }
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

Task("Publish-GitHub-Release")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldPublishGitHub)
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            if(DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages))
            {
                foreach(var package in GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*"))
                {
                    GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
                }
            }

            if(DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
            {
                foreach(var package in GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
                {
                    GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
                }
            }

            GitReleaseManagerClose(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone);
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
)
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});