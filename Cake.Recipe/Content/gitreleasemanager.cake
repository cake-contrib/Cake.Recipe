///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CreateReleaseNotesTask = Task("Create-Release-Notes")
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerCreate(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, new GitReleaseManagerCreateSettings {
                Milestone         = BuildParameters.Version.Milestone,
                Name              = BuildParameters.Version.Milestone,
                Prerelease        = false,
                TargetCommitish   = "master"
            });
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

BuildParameters.Tasks.ExportReleaseNotesTask = Task("Export-Release-Notes")
    .WithCriteria(() => BuildParameters.ShouldDownloadMilestoneReleaseNotes || BuildParameters.ShouldDownloadFullReleaseNotes)
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceLocalPublish)
    .WithCriteria(() => !BuildParameters.IsPullRequest || BuildParameters.ForceLocalPublish)
    .WithCriteria(() => BuildParameters.IsMainRepository || BuildParameters.ForceLocalPublish)
    .WithCriteria(() => BuildParameters.IsMasterBranch || BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch || BuildParameters.ForceLocalPublish)
    .WithCriteria(() => BuildParameters.IsTagged || BuildParameters.ForceLocalPublish)
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

BuildParameters.Tasks.PublishGitHubReleaseTask = Task("Publish-GitHub-Release")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldPublishGitHub)
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            // Concatenating FilePathCollections should make sure we get unique FilePaths
            foreach(var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                                   GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*") +
                                   GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
            {
                GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
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
