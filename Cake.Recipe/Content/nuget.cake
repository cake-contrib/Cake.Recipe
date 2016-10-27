Task("Create-NuGet-Packages")
    .IsDependentOn("Build")
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.NugetNuspecDirectory))
    .Does(() =>
{
    var nuspecFiles = GetFiles(parameters.Paths.Directories.NugetNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(parameters.Paths.Directories.NuGetPackages);

    foreach(var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = parameters.ReleaseNotes.Notes.ToArray(),

        if(DirectoryExists(parameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = parameters.Version.SemVersion,
                BasePath = parameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString()),
                OutputDirectory = parameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });

            continue;
        }

        if(DirectoryExists(parameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = parameters.Version.SemVersion,
                BasePath = parameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString()),
                OutputDirectory = parameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });

            continue;
        }

            // Create packages.
            NuGetPack(nuspecFile, new NuGetPackSettings {
                Version = parameters.Version.SemVersion,
                OutputDirectory = parameters.Paths.Directories.NuGetPackages,
                Symbols = false,
                NoPackageAnalysis = true
            });
    }
});

Task("Publish-MyGet-Packages")
    .IsDependentOn("Package")
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMainRepository)
    .WithCriteria(() => parameters.IsTagged || !parameters.IsMasterBranch)
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.NuGetPackages) || DirectoryExists(parameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    if(string.IsNullOrEmpty(parameters.MyGet.ApiKey)) {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    if(string.IsNullOrEmpty(parameters.MyGet.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve MyGet API url.");
    }

    var nupkgFiles = GetFiles(parameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = parameters.MyGet.SourceUrl,
            ApiKey = parameters.MyGet.ApiKey
        });
    }

    nupkgFiles = GetFiles(parameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = parameters.MyGet.SourceUrl,
            ApiKey = parameters.MyGet.ApiKey
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
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMainRepository)
    .WithCriteria(() => parameters.IsMasterBranch)
    .WithCriteria(() => parameters.IsTagged)
    .WithCriteria(() => DirectoryExists(parameters.Paths.Directories.NuGetPackages))
    .Does(() =>
{
    if(string.IsNullOrEmpty(parameters.NuGet.ApiKey)) {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    if(string.IsNullOrEmpty(parameters.NuGet.SourceUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    var nupkgFiles = GetFiles(parameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");

    foreach(var nupkgFile in nupkgFiles)
    {
        // Push the package.
        NuGetPush(nupkgFile, new NuGetPushSettings {
            Source = parameters.NuGet.SourceUrl,
            ApiKey = parameters.NuGet.ApiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Nuget-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});