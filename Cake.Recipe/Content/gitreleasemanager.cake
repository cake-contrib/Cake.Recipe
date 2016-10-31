///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Create-Release-Notes")
    .Does(() =>
{
    GitReleaseManagerCreate(parameters.GitHub.UserName, parameters.GitHub.Password, repositoryOwner, repositoryName, new GitReleaseManagerCreateSettings {
        Milestone         = parameters.Version.Milestone,
        Name              = parameters.Version.Milestone,
        Prerelease        = true,
        TargetCommitish   = "master"
    });
});

Task("Publish-GitHub-Release")
    .IsDependentOn("Package")
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMainRepository)
    .WithCriteria(() => parameters.IsMasterBranch)
    .WithCriteria(() => parameters.IsTagged)
    .Does(() =>
{
    if(DirectoryExists(parameters.Paths.Directories.NuGetPackages))
    {
        foreach(var package in GetFiles(parameters.Paths.Directories.NuGetPackages + "/*"))
        {
            GitReleaseManagerAddAssets(parameters.GitHub.UserName, parameters.GitHub.Password, repositoryOwner, repositoryName, parameters.Version.Milestone, package.ToString());
        }
    }

    if(DirectoryExists(parameters.Paths.Directories.ChocolateyPackages))
    {
        foreach(var package in GetFiles(parameters.Paths.Directories.ChocolateyPackages + "/*"))
        {
            GitReleaseManagerAddAssets(parameters.GitHub.UserName, parameters.GitHub.Password, repositoryOwner, repositoryName, parameters.Version.Milestone, package.ToString());
        }
    }

    GitReleaseManagerClose(parameters.GitHub.UserName, parameters.GitHub.Password, repositoryOwner, repositoryName, parameters.Version.Milestone);
})
.OnError(exception =>
{
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});