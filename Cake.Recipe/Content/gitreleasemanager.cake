///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Create-Release-Notes")
    .Does(() =>
{
    GitReleaseManagerCreate(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, repositoryOwner, repositoryName, new GitReleaseManagerCreateSettings {
        Milestone         = BuildParameters.Version.Milestone,
        Name              = BuildParameters.Version.Milestone,
        Prerelease        = true,
        TargetCommitish   = "master"
    });
});

Task("Publish-GitHub-Release")
    .IsDependentOn("Package")
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .WithCriteria(() => BuildParameters.IsMainRepository)
    .WithCriteria(() => BuildParameters.IsMasterBranch)
    .WithCriteria(() => BuildParameters.IsTagged)
    .Does(() =>
{
    if(DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages))
    {
        foreach(var package in GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*"))
        {
            GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, repositoryOwner, repositoryName, BuildParameters.Version.Milestone, package.ToString());
        }
    }

    if(DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    {
        foreach(var package in GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*"))
        {
            GitReleaseManagerAddAssets(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, repositoryOwner, repositoryName, BuildParameters.Version.Milestone, package.ToString());
        }
    }

    GitReleaseManagerClose(BuildParameters.GitHub.UserName, BuildParameters.GitHub.Password, repositoryOwner, repositoryName, BuildParameters.Version.Milestone);
})
.OnError(exception =>
{
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});