///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.WyamPublishDirectoryPath);
});

Task("Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation)
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath))
    .Does(() =>
{
    // Check to see if any documentation has changed
    var sourceCommit = GitLogTip("./");
    Information("Source Commit Sha: {0}", sourceCommit.Sha);
    var filesChanged = GitDiff("./", sourceCommit.Sha);
    Information("Number of changed files: {0}", filesChanged.Count);
    var docFileChanged = false;

    var wyamDocsFolderDirectoryName = BuildParameters.WyamRootDirectoryPath.GetDirectoryName();
    
    foreach(var file in filesChanged)
    {
        var backslash = '\\';
        Verbose("Changed File OldPath: {0}, Path: {1}", file.OldPath, file.Path);
        if(file.OldPath.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, backslash)) || 
            file.Path.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, backslash)) ||
            file.Path.Contains("config.wyam"))
        {
           docFileChanged = true;
           break; 
        }
    }

    if(docFileChanged)
    {
        Information("Detected that documentation files have changed, so running Wyam...");
        
        Wyam(new WyamSettings
        {
            Recipe = BuildParameters.WyamRecipe,
            Theme = BuildParameters.WyamTheme,
            OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
            RootPath = BuildParameters.WyamRootDirectoryPath,
            ConfigurationFile = BuildParameters.WyamConfigurationFile,
            PreviewVirtualDirectory = BuildParameters.WebLinkRoot,
            Settings = new Dictionary<string, object>
            {
                { "Host",  BuildParameters.WebHost },
                { "LinkRoot",  BuildParameters.WebLinkRoot },
                { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
                { "SourceFiles", BuildParameters.WyamSourceFiles }
            }
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
    Error(exception.Message);
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Preview-Documentation")
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath))
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = BuildParameters.WyamRecipe,
        Theme = BuildParameters.WyamTheme,
        OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
        RootPath = BuildParameters.WyamRootDirectoryPath,
        Preview = true,
        Watch = true,
        ConfigurationFile = BuildParameters.WyamConfigurationFile,
        PreviewVirtualDirectory = BuildParameters.WebLinkRoot,
        Settings = new Dictionary<string, object>
        {
            { "Host",  BuildParameters.WebHost },
            { "LinkRoot",  BuildParameters.WebLinkRoot },
            { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
            { "SourceFiles", BuildParameters.WyamSourceFiles }
        }
    });        
});

Task("Force-Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath))
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = BuildParameters.WyamRecipe,
        Theme = BuildParameters.WyamTheme,
        OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
        RootPath = BuildParameters.WyamRootDirectoryPath,
        ConfigurationFile = BuildParameters.WyamConfigurationFile,
        PreviewVirtualDirectory = BuildParameters.WebLinkRoot,
        Settings = new Dictionary<string, object>
        {
            { "Host",  BuildParameters.WebHost },
            { "LinkRoot",  BuildParameters.WebLinkRoot },
            { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
            { "SourceFiles", BuildParameters.WyamSourceFiles }
        }
    });

    PublishDocumentation();
});

public void PublishDocumentation()
{
    if(BuildParameters.CanUseWyam)
    {
        var sourceCommit = GitLogTip("./");

        var publishFolder = BuildParameters.WyamPublishDirectoryPath.Combine(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        Information("Publishing Folder: {0}", publishFolder);
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
