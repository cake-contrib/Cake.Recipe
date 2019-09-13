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
                TargetCommitish   = BuildParameters.MasterBranchName,
                Prerelease        = false
            });
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);

BuildParameters.Tasks.ExportReleaseNotesTask = Task("Export-Release-Notes")
    .WithCriteria(() => BuildParameters.ShouldDownloadMilestoneReleaseNotes || BuildParameters.ShouldDownloadFullReleaseNotes, "Exporting Release notes has been disabled")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.PrepareLocalRelease, "Is local build, and is not preparing local release")
    .WithCriteria(() => !BuildParameters.IsPullRequest || BuildParameters.PrepareLocalRelease, "Is pull request, and is not preparing local release")
    .WithCriteria(() => BuildParameters.IsMainRepository || BuildParameters.PrepareLocalRelease, "Is not main repository, and is not preparing local release")
    .WithCriteria(() => BuildParameters.IsMasterBranch || BuildParameters.IsReleaseBranch || BuildParameters.IsHotFixBranch || BuildParameters.PrepareLocalRelease, "Is not a releasable branch, and is not preparing local release")
    .WithCriteria(() => BuildParameters.IsTagged || BuildParameters.PrepareLocalRelease, "Is not a tagged build, and is not preparing local release")
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

BuildParameters.Tasks.CreateDefaultLabelsTask = Task("Create-Default-Labels")
    .Does(() => RequireTool(GitReleaseManagerTool, () => {
        if(BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerLabel(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);
