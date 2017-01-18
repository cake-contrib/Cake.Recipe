///////////////////////////////////////////////////////////////////////////////
// VARIABLES 
///////////////////////////////////////////////////////////////////////////////

var shouldPublishDocumentation = false;

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .WithCriteria(BuildParameters.IsRunningOnAppVeyor)
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.WyamPublishDirectoryPath);
});

Task("Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation)
    .Does(() =>
{
    // Check to see if any documentation has changed
    var sourceCommit = GitLogTip("./");
    var filesChanged = GitDiff("./", sourceCommit.Sha);
    var docFileChanged = false;

    foreach(var file in filesChanged)
    {
        if(file.OldPath.Contains("docs/input") || file.Path.Contains("docs/input"))
        {
           docFileChanged = true;
           break; 
        }
    }

    if(docFileChanged)
    {
        Wyam(new WyamSettings
        {
            Recipe = BuildParameters.WyamRecipe,
            Theme = BuildParameters.WyamTheme,
            OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
            RootPath = BuildParameters.WyamRootDirectoryPath
        });

        PublishDocumentation();
    }
    else
    {
        Information("No documentation has changed, so no need to generate documentation");
    }
})
.OnError(exception =>
{
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
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

public void PublishDocumentation()
{
    if(BuildParameters.CanUseWyam)
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
    }
    else
    {
        Warning("Unable to publish documentation, as not all Wyam Configuration is present");
    }
}