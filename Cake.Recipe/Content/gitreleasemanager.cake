///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CreateReleaseNotesTask = Task("Create-Release-Notes")
    .Does(() => RequireTool(ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            var settings = new GitReleaseManagerCreateSettings
            {
                Milestone         = BuildParameters.Version.Milestone,
                Name              = BuildParameters.Version.Milestone,
                TargetCommitish   = BuildParameters.MasterBranchName,
                Prerelease        = false
            };

            if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
            {
                GitReleaseManagerCreate(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, settings);
            }
            else
            {
                GitReleaseManagerCreate(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, settings);
            }
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
    .WithCriteria(() => BuildParameters.BranchType == BranchType.Master || BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix || BuildParameters.PrepareLocalRelease, "Is not a releasable branch, and is not preparing local release")
    .WithCriteria(() => BuildParameters.IsTagged || BuildParameters.PrepareLocalRelease, "Is not a tagged build, and is not preparing local release")
    .Does(() => RequireTool(ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            if (BuildParameters.ShouldDownloadMilestoneReleaseNotes)
            {
                var settings = new GitReleaseManagerExportSettings
                {
                    TagName         = BuildParameters.Version.Milestone
                };

                if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
                {
                    GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.MilestoneReleaseNotesFilePath, settings);
                }
                else
                {
                    GitReleaseManagerExport(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.MilestoneReleaseNotesFilePath, settings);
                }
            }

            if (BuildParameters.ShouldDownloadFullReleaseNotes)
            {
                if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
                {
                    GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.FullReleaseNotesFilePath);
                }
                else
                {
                    GitReleaseManagerExport(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.FullReleaseNotesFilePath);
                }
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
    .Does(() => RequireTool(ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            // Concatenating FilePathCollections should make sure we get unique FilePaths
            foreach (var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                                   GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*") +
                                   GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
            {
                if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
                {
                    GitReleaseManagerAddAssets(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
                }
                else
                {
                    GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone, package.ToString());
                }
            }

            if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
            {
                GitReleaseManagerClose(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone);
            }
            else
            {
                GitReleaseManagerClose(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.Version.Milestone);
            }
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
    .Does(() => RequireTool(ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            if (!string.IsNullOrEmpty(BuildParameters.GitHub.Token))
            {
                GitReleaseManagerLabel(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
            }
            else
            {
                GitReleaseManagerLabel(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
            }
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);
