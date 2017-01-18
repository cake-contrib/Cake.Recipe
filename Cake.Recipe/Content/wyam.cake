///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .WithCriteria(BuildParameters.IsRunningOnAppVeyor)
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.WyamPublishDirectoryPath);
});

Task("Build-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation)
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = BuildParameters.WyamRecipe,
        Theme = BuildParameters.WyamTheme,
        OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
        RootPath = BuildParameters.WyamRootDirectoryPath
    });        
});

Task("Preview-Documentation")
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = BuildParameters.WyamRecipe,
        Theme = BuildParameters.WyamTheme,
        OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
        RootPath = BuildParameters.WyamRootDirectoryPath,
        Preview = true,
        Watch = true
    });        
});

Task("Publish-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation)
    .IsDependentOn("Build-Documentation")
    .Does(() =>
{
    var sourceCommit = GitLogTip("./");
    var publishFolder = BuildParameters.WyamPublishDirectoryPath.Combine(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
    Information("Getting publish branch...");
    GitClone(BuildParameters.Wyam.DeployRemote, publishFolder, new GitCloneSettings{ BranchName = BuildParameters.Wyam.DeployBranch });

    Information("Sync output files...");
    Kudu.Sync(BuildParameters.Paths.Directories.PublishedDocumentation, publishFolder, new KuduSyncSettings { 
        ArgumentCustomization = args=>args.Append("--ignore").AppendQuoted(".git;CNAME")
    });

    Information("Stage all changes...");
    GitAddAll(publishFolder);

    Information("Commit all changes...");
    GitCommit(
        publishFolder,
        sourceCommit.Committer.Name,
        sourceCommit.Committer.Email,
        string.Format("AppVeyor Publish: {0}\r\n{1}", sourceCommit.Sha, sourceCommit.Message)
        );

    Information("Pushing all changes...");
    GitPush(publishFolder, BuildParameters.Wyam.AccessToken, "x-oauth-basic", BuildParameters.Wyam.DeployBranch);
})
.OnError(exception =>
{
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
});