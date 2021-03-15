///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CleanDocumentationTask = Task("Clean-Documentation")
    .Does(() =>
{
    EnsureDirectoryExists(BuildParameters.WyamPublishDirectoryPath);
});

BuildParameters.Tasks.PublishDocumentationTask = Task("Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation, "Wyam documentation has been disabled")
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath), "Wyam documentation directory is missing")
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.WyamGlobalTool : ToolSettings.WyamTool, () => {
        // Check to see if any documentation has changed
        var sourceCommit = GitLogTip("./");
        Information("Source Commit Sha: {0}", sourceCommit.Sha);
        var filesChanged = GitDiff("./", sourceCommit.Sha);
        Information("Number of changed files: {0}", filesChanged.Count);
        var docFileChanged = false;

        var wyamDocsFolderDirectoryName = BuildParameters.WyamRootDirectoryPath.GetDirectoryName();

        var pathsToTestAgainst = new List<string>() {
            string.Format("{0}{1}", wyamDocsFolderDirectoryName, '/')
        };

        if (BuildParameters.ShouldDocumentSourceFiles)
        {
            // BuildParameters.WyamSourceFiles can not be used - the wyam globs are different from globs in GetFiles().
            pathsToTestAgainst.Add(string.Format("{0}{1}", BuildParameters.SourceDirectoryPath.FullPath, '/'));
        }

        Verbose("Comparing all file-changes to the following paths:");
        foreach(var p in pathsToTestAgainst)
        {
            Verbose(" - "+p);
        }

        foreach (var file in filesChanged)
        {
            Verbose("Changed File OldPath: {0}, Path: {1}", file.OldPath, file.Path);
            if (pathsToTestAgainst.Any(x => file.OldPath.Contains(x) || file.Path.Contains(x)) ||
                file.Path.Contains("config.wyam"))
            {
            docFileChanged = true;
            break;
            }
        }

        if (docFileChanged)
        {
            Information("Detected that documentation files have changed, so running Wyam...");
            var settings = new Dictionary<string, object>
            {
                { "Host",  BuildParameters.WebHost },
                { "LinkRoot",  BuildParameters.WebLinkRoot },
                { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
                { "Title", BuildParameters.Title },
                { "IncludeGlobalNamespace", false }
            };

            if (BuildParameters.ShouldDocumentSourceFiles)
            {
                settings.Add("SourceFiles", BuildParameters.WyamSourceFiles);
            }

            Wyam(new WyamSettings
            {
                Recipe = BuildParameters.WyamRecipe,
                Theme = BuildParameters.WyamTheme,
                OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
                RootPath = BuildParameters.WyamRootDirectoryPath,
                ConfigurationFile = BuildParameters.WyamConfigurationFile,
                PreviewVirtualDirectory = BuildParameters.WebLinkRoot,
                Settings = settings
            });

            PublishDocumentation();
        }
        else
        {
            Information("No documentation has changed, so no need to generate documentation");
        }
    })
)
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.PreviewDocumentationTask = Task("Preview-Documentation")
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath), "Wyam documentation directory is missing")
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.WyamGlobalTool : ToolSettings.WyamTool, () => {
        var settings = new Dictionary<string, object>
        {
            { "Host",  BuildParameters.WebHost },
            { "LinkRoot",  BuildParameters.WebLinkRoot },
            { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
            { "Title", BuildParameters.Title },
            { "IncludeGlobalNamespace", false }
        };

        if (BuildParameters.ShouldDocumentSourceFiles)
        {
            settings.Add("SourceFiles", BuildParameters.WyamSourceFiles);
        }

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
            Settings = settings
        });
    })
);

BuildParameters.Tasks.ForcePublishDocumentationTask = Task("Force-Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => DirectoryExists(BuildParameters.WyamRootDirectoryPath), "Wyam documentation directory is missing")
    .Does(() => RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.WyamGlobalTool : ToolSettings.WyamTool, () => {
        var settings = new Dictionary<string, object>
        {
            { "Host",  BuildParameters.WebHost },
            { "LinkRoot",  BuildParameters.WebLinkRoot },
            { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
            { "Title", BuildParameters.Title },
            { "IncludeGlobalNamespace", false }
        };

        if (BuildParameters.ShouldDocumentSourceFiles)
        {
            settings.Add("SourceFiles", BuildParameters.WyamSourceFiles);
        }

        Wyam(new WyamSettings
        {
            Recipe = BuildParameters.WyamRecipe,
            Theme = BuildParameters.WyamTheme,
            OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
            RootPath = BuildParameters.WyamRootDirectoryPath,
            ConfigurationFile = BuildParameters.WyamConfigurationFile,
            PreviewVirtualDirectory = BuildParameters.WebLinkRoot,
            Settings = settings
        });

        PublishDocumentation();
    })
);

public void PublishDocumentation()
{
    RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.KuduSyncGlobalTool : ToolSettings.KuduSyncTool, () => {
        if (BuildParameters.CanUseWyam)
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

            if (GitHasUncommitedChanges(publishFolder))
            {
                Information("Stage all changes...");
                GitAddAll(publishFolder);

                if (GitHasStagedChanges(publishFolder))
                {
                    Information("Commit all changes...");
                    GitCommit(
                        publishFolder,
                        sourceCommit.Committer.Name,
                        sourceCommit.Committer.Email,
                        string.Format("Continuous Integration Publish: {0}\r\n{1}", sourceCommit.Sha, sourceCommit.Message)
                    );

                    Information("Pushing all changes...");
                    GitPush(publishFolder, BuildParameters.Wyam.AccessToken, "x-oauth-basic", BuildParameters.Wyam.DeployBranch);
                }
            }
        }
        else
        {
            Warning("Unable to publish documentation, as not all Wyam Configuration is present");
        }
    });
}
