Task("Create-Chocolatey-Packages")
    .IsDependentOn("Build")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.ChocolateyNuspecDirectory))
    .Does(() =>
{
    var nuspecFiles = GetFiles(parameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(parameters.Paths.Directories.ChocolateyPackages);

    foreach(var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = parameters.ReleaseNotes.Notes.ToArray(),

        // Create package.
        ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
            Version = parameters.Version.SemVersion,
            OutputDirectory = parameters.Paths.Directories.ChocolateyPackages,
            WorkingDirectory = parameters.Paths.Directories.PublishedApplications
        });
    }
});

Task("Publish-Chocolatey-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMainRepository)
    .WithCriteria(() => parameters.IsMasterBranch)
    .WithCriteria(() => parameters.IsTagged)
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    if(string.IsNullOrEmpty(parameters.Chocolatey.ApiKey)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API key.");
    }

    if(string.IsNullOrEmpty(parameters.Chocolatey.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API url.");
    }

    var nupkgFiles = GetFiles(parameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        ChocolateyPush(nupkgFile, new ChocolateyPushSettings {
          ApiKey = parameters.Chocolatey.ApiKey,
          Source = parameters.Chocolatey.SourceUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Chocolatey-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});