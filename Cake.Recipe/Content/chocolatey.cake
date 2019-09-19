BuildParameters.Tasks.CreateChocolateyPackagesTask = Task("Create-Chocolatey-Packages")
    .IsDependentOn("Clean")
    .WithCriteria(() => BuildParameters.ShouldRunChocolatey, "Skipping because execution of Chocolatey has been disabled")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows, "Skipping because not running on Windows")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory), "Skipping because Chocolatey nuspec directory is missing")
    .Does(() =>
{
    var nuspecFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages);

    foreach(var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(),

        // Create package.
        ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
            Version = BuildParameters.Version.SemVersion,
            OutputDirectory = BuildParameters.Paths.Directories.ChocolateyPackages,
            WorkingDirectory = BuildParameters.Paths.Directories.PublishedApplications
        });
    }
});

BuildParameters.Tasks.PublishChocolateyPackagesTask = Task("Publish-Chocolatey-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldRunChocolatey, "Skipping because execution of Chocolatey has been disabled")
    .WithCriteria(() => BuildParameters.IsRunningOnWindows, "Skipping because not running on Windows")
    .WithCriteria(() => BuildParameters.ShouldPublishChocolatey, "Publishing Chocolatey packages has been disabled")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages), "Skipping because no Chocolatey package(s) have been created")
    .Does(() =>
{
    if(BuildParameters.CanPublishToChocolatey)
    {
        var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

        foreach(var nupkgFile in nupkgFiles)
        {
            // Push the package.
            ChocolateyPush(nupkgFile, new ChocolateyPushSettings {
            ApiKey = BuildParameters.Chocolatey.ApiKey,
            Source = BuildParameters.Chocolatey.SourceUrl
            });
        }
    }
    else
    {
        Warning("Unable to publish to Chocolatey, as necessary credentials are not available");
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Chocolatey-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});
