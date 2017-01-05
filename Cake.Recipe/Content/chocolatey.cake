Task("Create-Chocolatey-Packages")
    .IsDependentOn("Build")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory))
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

Task("Publish-Chocolatey-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldPublishChocolatey)
    .Does(() =>
{
    if(string.IsNullOrEmpty(BuildParameters.Chocolatey.ApiKey)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API key.");
    }

    if(string.IsNullOrEmpty(BuildParameters.Chocolatey.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API url.");
    }

    var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        ChocolateyPush(nupkgFile, new ChocolateyPushSettings {
          ApiKey = BuildParameters.Chocolatey.ApiKey,
          Source = BuildParameters.Chocolatey.SourceUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Chocolatey-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});