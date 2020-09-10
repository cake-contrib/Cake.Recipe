///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CreateReleaseNotesTask = Task("Create-Release-Notes")
    .Does<BuildVersion>((context, buildVersion) => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            var settings = new GitReleaseManagerCreateSettings
            {
                Milestone         = buildVersion.Milestone,
                Name              = buildVersion.Milestone,
                TargetCommitish   = BuildParameters.MasterBranchName,
                Prerelease        = context.HasArgument("create-pre-release")
            };
            if (settings.Prerelease)
            {
                settings.TargetCommitish = BuildParameters.BuildProvider.Repository.Branch;
            }

            GitReleaseManagerCreate(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, settings);
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
    .WithCriteria(() => BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem, "Not running on preferred build agent operating system")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .Does<BuildVersion>((context, buildVersion) => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            if (BuildParameters.ShouldDownloadMilestoneReleaseNotes)
            {
                var settings = new GitReleaseManagerExportSettings
                {
                    TagName         = buildVersion.Milestone
                };

                GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.MilestoneReleaseNotesFilePath, settings);
            }

            if (BuildParameters.ShouldDownloadFullReleaseNotes)
            {
                GitReleaseManagerExport(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, BuildParameters.FullReleaseNotesFilePath);
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
    .Does<BuildVersion>((context, buildVersion) => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            // Concatenating FilePathCollections should make sure we get unique FilePaths
            foreach (var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                                   GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*") +
                                   GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
            {
                GitReleaseManagerAddAssets(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, buildVersion.Milestone, package.ToString());
            }

            GitReleaseManagerClose(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName, buildVersion.Milestone);
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
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.GitReleaseManagerGlobalTool : ToolSettings.GitReleaseManagerTool, () => {
        if (BuildParameters.CanUseGitReleaseManager)
        {
            GitReleaseManagerLabel(BuildParameters.GitHub.Token, BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
        }
        else
        {
            Warning("Unable to use GitReleaseManager, as necessary credentials are not available");
        }
    })
);
