Task("Create-NuGet-Packages")
    .IsDependentOn("Build")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.NugetNuspecDirectory))
    .Does(() =>
{
    var nuspecFiles = GetFiles(BuildParameters.Paths.Directories.NugetNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.NuGetPackages);

    foreach(var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(),

        if(DirectoryExists(BuildParameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = BuildParameters.Version.SemVersion,
                BasePath = BuildParameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString()),
                OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });

            continue;
        }

        if(DirectoryExists(BuildParameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = BuildParameters.Version.SemVersion,
                BasePath = BuildParameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString()),
                OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });

            continue;
        }

            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = BuildParameters.Version.SemVersion,
                OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });
    }
});

Task("Publish-MyGet-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldPublishMyGet)
    .Does(() =>
{
    if(string.IsNullOrEmpty(BuildParameters.MyGet.ApiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    if(string.IsNullOrEmpty(BuildParameters.MyGet.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve MyGet API url.");
    }

    var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = BuildParameters.MyGet.SourceUrl,
            ApiKey = BuildParameters.MyGet.ApiKey
        });
    }

    nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = BuildParameters.MyGet.SourceUrl,
            ApiKey = BuildParameters.MyGet.ApiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-MyGet-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Publish-Nuget-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.ShouldPublishNuGet)
    .Does(() =>
{
    if(string.IsNullOrEmpty(BuildParameters.NuGet.ApiKey)) {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    if(string.IsNullOrEmpty(BuildParameters.NuGet.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = BuildParameters.NuGet.SourceUrl,
            ApiKey = BuildParameters.NuGet.ApiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Nuget-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});