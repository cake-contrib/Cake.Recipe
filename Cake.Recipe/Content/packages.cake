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

BuildParameters.Tasks.DotNetCorePackTask = Task("DotNetCore-Pack")
    .IsDependentOn("DotNetCore-Build")
    .WithCriteria(() => BuildParameters.ShouldRunDotNetCorePack, "Packaging through .NET Core is disabled")
    .Does(() =>
{
    var projects = GetFiles(BuildParameters.SourceDirectoryPath + "/**/*.csproj")
        - GetFiles(BuildParameters.RootDirectoryPath + "/tools/**/*.csproj")
        - GetFiles(BuildParameters.SourceDirectoryPath + "/**/*.Tests.csproj")
        - GetFiles(BuildParameters.SourceDirectoryPath + "/packages/**/*.csproj");

    var msBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", BuildParameters.Version.SemVersion)
                            .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
                            .WithProperty("FileVersion",  BuildParameters.Version.Version)
                            .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

    if(!IsRunningOnWindows())
    {
        var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        // Use FrameworkPathOverride when not running on Windows.
        Information("Pack will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
        msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }

    var settings = new DotNetCorePackSettings {
        NoBuild = true,
        NoRestore = true,
        Configuration = BuildParameters.Configuration,
        OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
        MSBuildSettings = msBuildSettings,
        ArgumentCustomization = (args) => {
            if (BuildParameters.ShouldBuildNugetSourcePackage)
            {
                args.Append("--include-source");
            }
            return args;
        }
    };

    foreach (var project in projects)
    {
        DotNetCorePack(project.ToString(), settings);
    }
});

BuildParameters.Tasks.CreateNuGetPackageTask = Task("Create-Nuget-Package")
    .IsDependentOn("Clean")
    .Does(() =>
{
    if(BuildParameters.NuSpecFilePath != null) {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.NuGetPackages);

        // Create packages.
        NuGetPack(BuildParameters.NuSpecFilePath, new NuGetPackSettings {
            Version = BuildParameters.Version.SemVersion,
            OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
            Symbols = false,
            NoPackageAnalysis = true
        });
    }
    else
    {
        throw new Exception("NuSpecFilePath has not been set");
    }
});

BuildParameters.Tasks.CreateNuGetPackagesTask = Task("Create-NuGet-Packages")
    .IsDependentOn("Clean")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.NugetNuspecDirectory), "NuGet nuspec directory does not exist")
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

BuildParameters.Tasks.PublishPackagesTask = Task("Publish-Packages")
    .IsDependentOn("Package")
    .Does(() =>
{
    var chocolateySources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.Chocolatey);
    var nugetSources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.NuGet);

    if(BuildParameters.ShouldRunChocolatey && BuildParameters.IsRunningOnWindows && BuildParameters.ShouldPublishChocolatey && DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    {
        foreach(var chocolateySource in chocolateySources)
        {
            var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/**/*.nupkg");

            foreach(var nupkgFile in nupkgFiles)
            {
                // Push the package.
                ChocolateyPush(nupkgFile, new ChocolateyPushSettings {
                    ApiKey = chocolateySource.Credentials.ApiKey,
                    Source = chocolateySource.PushUrl
                });
            }
        }
    }

    if(DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages))
    {
        foreach(var nugetSource in nugetSources)
        {
            var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/**/*.nupkg");
            var nugetPushSettings = new NuGetPushSettings
                {
                    Source = nugetSource.PushUrl
                };

            if(!string.IsNullOrEmpty(nugetSource.Credentials.ApiKey))
            {
                Information("Setting ApiKey in NuGet Push Settings...");
                nugetPushSettings.ApiKey = nugetSource.Credentials.ApiKey;
            }
            else
            {
                var nugetSourceSettings = new NuGetSourcesSettings
                    {
                        UserName = nugetSource.Credentials.User,
                        Password = nugetSource.Credentials.Password
                    };

                Information("Adding NuGet source with user/pass...");
                NuGetAddSource("PreReleaseSource", nugetSource.PushUrl, nugetSourceSettings);
            }

            foreach(var nupkgFile in nupkgFiles)
            {
                // Push the package.
                NuGetPush(nupkgFile, nugetPushSettings);
            }
        }
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});