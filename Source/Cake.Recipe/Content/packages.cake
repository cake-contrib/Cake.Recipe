BuildParameters.Tasks.CreateChocolateyPackagesTask = Task("Create-Chocolatey-Packages")
    .IsDependentOn("Clean")
    .WithCriteria(() => BuildParameters.ShouldRunChocolatey, "Skipping because execution of Chocolatey has been disabled")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping because not running on Windows")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory), "Skipping because Chocolatey nuspec directory is missing")
    .Does<BuildVersion>((context, buildVersion) =>
{
    var nuspecFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages);

    foreach (var nuspecFile in nuspecFiles)
    {
        // TODO: Addin the release notes
        // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(),

        // Create package.
        ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
            Version = buildVersion.SemVersion,
            OutputDirectory = BuildParameters.Paths.Directories.ChocolateyPackages,
            WorkingDirectory = BuildParameters.Paths.Directories.PublishedApplications
        });
    }
});

BuildParameters.Tasks.DotNetCorePackTask = Task("DotNetCore-Pack")
    .IsDependentOn("DotNetCore-Build")
    .WithCriteria(() => BuildParameters.ShouldRunDotNetCorePack, "Packaging through .NET Core is disabled")
    .Does<BuildVersion>((context, buildVersion) =>
{
    var projects = GetFiles(BuildParameters.SourceDirectoryPath + "/**/*.csproj")
        - GetFiles(BuildParameters.RootDirectoryPath + "/tools/**/*.csproj")
        - GetFiles(BuildParameters.SourceDirectoryPath + "/**/*.Tests.csproj")
        - GetFiles(BuildParameters.SourceDirectoryPath + "/packages/**/*.csproj");

    // We need to clone the settings class, so we don't
    // add additional properties to every other task.
    var msBuildSettings = new DotNetCoreMSBuildSettings();
    foreach (var kv in context.Data.Get<DotNetCoreMSBuildSettings>().Properties)
    {
        string value = string.Join(" ", kv.Value);
        msBuildSettings.WithProperty(kv.Key, value);
    }

    if (BuildParameters.ShouldBuildNugetSourcePackage)
    {
        msBuildSettings.WithProperty("SymbolPackageFormat", "snupkg");
    }

    var settings = new DotNetCorePackSettings {
        NoBuild = true,
        NoRestore = true,
        Configuration = BuildParameters.Configuration,
        OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
        MSBuildSettings = msBuildSettings,
        IncludeSource = BuildParameters.ShouldBuildNugetSourcePackage,
        IncludeSymbols = BuildParameters.ShouldBuildNugetSourcePackage,
    };

    foreach (var project in projects)
    {
        DotNetCorePack(project.ToString(), settings);
    }
});

BuildParameters.Tasks.CreateNuGetPackageTask = Task("Create-Nuget-Package")
    .IsDependentOn("Clean")
    .Does<BuildVersion>((context, buildVersion) =>
{
    if (BuildParameters.NuSpecFilePath != null) {
        EnsureDirectoryExists(BuildParameters.Paths.Directories.NuGetPackages);

        RequireToolNotRegistered(ToolSettings.NuGetTool, new[] { "nuget", "nuget.exe" }, () => {
            // Create packages.
            NuGetPack(BuildParameters.NuSpecFilePath, new NuGetPackSettings {
                Version = buildVersion.SemVersion,
                OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
                Symbols = BuildParameters.ShouldBuildNugetSourcePackage,
                NoPackageAnalysis = true,
                ArgumentCustomization = args => {
                    if (BuildParameters.ShouldBuildNugetSourcePackage)
                        return args.AppendSwitch("-SymbolPackageFormat", "snupkg");
                    else
                        return args;
                }
            });
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
    .Does<BuildVersion>((context, buildVersion) =>
{
    var nuspecFiles = GetFiles(BuildParameters.Paths.Directories.NugetNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.NuGetPackages);

    var settings = new NuGetPackSettings {
        Version = buildVersion.SemVersion,
        OutputDirectory = BuildParameters.Paths.Directories.NuGetPackages,
        Symbols = BuildParameters.ShouldBuildNugetSourcePackage,
        NoPackageAnalysis = true
    };
    if (BuildParameters.ShouldBuildNugetSourcePackage)
    {
        settings.ArgumentCustomization = args => args.AppendSwitch("-SymbolPackageFormat", "snupkg");
    }

    RequireToolNotRegistered(ToolSettings.NuGetTool, new[] { "nuget", "nuget.exe" }, () => { });

    foreach (var nuspecFile in nuspecFiles)
    {
        settings.BasePath = null; // First blank out the settings base path
        // TODO: Addin the release notes
        // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(),

        if (DirectoryExists(BuildParameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            settings.BasePath = BuildParameters.Paths.Directories.PublishedLibraries.Combine(nuspecFile.GetFilenameWithoutExtension().ToString());
        }
        else if (DirectoryExists(BuildParameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString())))
        {
            settings.BasePath = BuildParameters.Paths.Directories.PublishedApplications.Combine(nuspecFile.GetFilenameWithoutExtension().ToString());
        }

        // Create packages.
        NuGetPack(nuspecFile, settings);
    }
});

BuildParameters.Tasks.PublishPreReleasePackagesTask = Task("Publish-PreRelease-Packages")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceContinuousIntegration, "Skipping because this is a local build, and force isn't being applied")
    .WithCriteria(() => !BuildParameters.IsPullRequest, "Skipping because current build is from a Pull Request")
    .WithCriteria(() => !BuildParameters.IsTagged, "Skipping because current commit is tagged")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .IsDependentOn("Package")
    .Does(() =>
{
    if (BuildParameters.PreferredBuildAgentOperatingSystem == PlatformFamily.Windows)
    {
        var chocolateySources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.Chocolatey && p.IsRelease == false).ToList();
        PushChocolateyPackages(Context, false, chocolateySources);
    }

    if (BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem)
    {
        var nugetSources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.NuGet && p.IsRelease == false).ToList();
        RequireToolNotRegistered(ToolSettings.NuGetTool, new[] { "nuget", "nuget.exe" }, () => {
            PushNuGetPackages(Context, false, nugetSources);
        });
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-PreRelease-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.PublishReleasePackagesTask = Task("Publish-Release-Packages")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceContinuousIntegration, "Skipping because this is a local build, and force isn't being applied")
    .WithCriteria(() => !BuildParameters.IsPullRequest, "Skipping because current build is from a Pull Request")
    .WithCriteria(() => BuildParameters.IsTagged, "Skipping because current commit is not tagged")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .IsDependentOn("Package")
    .Does(() =>
{
    if (BuildParameters.PreferredBuildAgentOperatingSystem == PlatformFamily.Windows)
    {
        var chocolateySources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.Chocolatey && p.IsRelease == true).ToList();
        PushChocolateyPackages(Context, true, chocolateySources);
    }

    if (BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem)
    {
        var nugetSources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.NuGet && p.IsRelease == true).ToList();
        RequireToolNotRegistered(ToolSettings.NuGetTool, new[] { "nuget", "nuget.exe" }, () => {
            PushNuGetPackages(Context, true, nugetSources);
        });
        // Only consider pushes to nuget and on the same Build Agent Operating System
        BuildParameters.PublishReleasePackagesWasSuccessful = true;
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Release-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});


public void PushChocolateyPackages(ICakeContext context, bool isRelease, List<PackageSourceData> chocolateySources)
{
    if (DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    {
        Information("Number of configured {0} Chocolatey Sources: {1}", isRelease ? "Release" : "PreRelease", chocolateySources.Count());

        foreach (var chocolateySource in chocolateySources)
        {
            var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*.nupkg");

            var chocolateyPushSettings = new ChocolateyPushSettings
                {
                    Source = chocolateySource.PushUrl
                };

            var canPushToChocolateySource = false;
            if (!string.IsNullOrEmpty(chocolateySource.Credentials.ApiKey))
            {
                context.Information("Setting ApiKey in Chocolatey Push Settings...");
                chocolateyPushSettings.ApiKey = chocolateySource.Credentials.ApiKey;
                canPushToChocolateySource = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(chocolateySource.Credentials.User) && !string.IsNullOrEmpty(chocolateySource.Credentials.Password))
                {
                    var chocolateySourceSettings = new ChocolateySourcesSettings
                    {
                        UserName = chocolateySource.Credentials.User,
                        Password = chocolateySource.Credentials.Password
                    };

                    context.Information("Adding Chocolatey source with user/pass...");
                    context.ChocolateyAddSource(isRelease ? string.Format("ReleaseSource_{0}", chocolateySource.Name) : string.Format("PreReleaseSource_{0}", chocolateySource.Name), chocolateySource.PushUrl, chocolateySourceSettings);
                    canPushToChocolateySource = true;
                }
                else
                {
                    context.Warning("User and Password are missing for {0} Chocolatey Source with Url {1}", isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);
                }
            }

            if (canPushToChocolateySource)
            {
                foreach (var nupkgFile in nupkgFiles)
                {
                    context.Information("Pushing {0} to {1} Source with Url {2}...", nupkgFile, isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);

                    // Push the package.
                    context.ChocolateyPush(nupkgFile, chocolateyPushSettings);
                }
            }
            else
            {
                context.Warning("Unable to push Chocolatey Packages to {0} Source with Url {1} as necessary credentials haven't been provided.", isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);
            }
        }
    }
    else
    {
        context.Information("Unable to publish Chocolatey packages. Chocolatey Packages Directory Exists: {0}", DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages));
    }
}

public void PushNuGetPackages(ICakeContext context, bool isRelease, List<PackageSourceData> nugetSources)
{
    if (DirectoryExists(BuildParameters.Paths.Directories.NuGetPackages))
    {
        context.Information("Number of configured {0} NuGet Sources: {1}", isRelease ? "Release" : "PreRelease", nugetSources.Count());

        foreach (var nugetSource in nugetSources)
        {
            var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*.nupkg");

            var nugetPushSettings = new NuGetPushSettings
                {
                    Source = nugetSource.PushUrl,
                    SkipDuplicate = ToolSettings.SkipDuplicatePackages
                };

            var canPushToNuGetSource = false;
            if (!string.IsNullOrEmpty(nugetSource.Credentials.ApiKey))
            {
                context.Information("Setting ApiKey in NuGet Push Settings...");
                nugetPushSettings.ApiKey = nugetSource.Credentials.ApiKey;
                canPushToNuGetSource = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(nugetSource.Credentials.User) && !string.IsNullOrEmpty(nugetSource.Credentials.Password))
                {
                    var nugetSourceSettings = new NuGetSourcesSettings
                        {
                            UserName = nugetSource.Credentials.User,
                            Password = nugetSource.Credentials.Password
                        };

                    if (nugetSource.Name == "AZURE")
                    {
                        // This is required so that the X-NuGet-ApiKey is set.  Currently,
                        // it is only known that this is required when pushing to Azure
                        // Artifacts, as such, a more general solution hasn't been provided.
                        nugetPushSettings.ApiKey = "RandomToken";
                    }

                    context.Information("Adding NuGet source with user/pass...");
                    context.NuGetAddSource(isRelease ? string.Format("ReleaseSource_{0}", nugetSource.Name) : string.Format("PreReleaseSource_{0}", nugetSource.Name), nugetSource.PushUrl, nugetSourceSettings);
                    canPushToNuGetSource = true;
                }
                else
                {
                    context.Warning("User and Password are missing for {0} NuGet Source with Url {1}", isRelease ? "Release" : "PreRelease", nugetSource.PushUrl);
                }
            }

            if (canPushToNuGetSource)
            {
                foreach (var nupkgFile in nupkgFiles)
                {
                    context.Information("Pushing {0} to {1} Source with Url {2}...", nupkgFile, isRelease ? "Release" : "PreRelease", nugetSource.PushUrl);

                    // Push the package.
                    context.NuGetPush(nupkgFile, nugetPushSettings);
                }
            }
            else
            {
                 context.Warning("Unable to push NuGet Packages to {0} Source with Url {1} as necessary credentials haven't been provided.", isRelease ? "Release" : "PreRelease", nugetSource.PushUrl);
            }
        }
    }
    else
    {
        context.Information("Unable to publish NuGet Packages as NuGet Packages Directory doesn't exist.");
    }
}
