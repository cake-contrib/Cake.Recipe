#if !CUSTOM_VERSIONING
Setup<BuildVersion>(context =>
{
    BuildVersion buildVersion = null;

    RequireTool(BuildParameters.IsDotNetCoreBuild ? ToolSettings.GitVersionGlobalTool : ToolSettings.GitVersionTool, () => {
        buildVersion = BuildVersion.CalculatingSemanticVersion(
                context: Context
            );
        });

    Information("Building version {0} of " + BuildParameters.Title + " ({1}, {2}) using version {3} of Cake, and version {4} of Cake.Recipe. (IsTagged: {5})",
        buildVersion.SemVersion,
        BuildParameters.Configuration,
        BuildParameters.Target,
        buildVersion.CakeVersion,
        BuildMetaData.Version,
        BuildParameters.IsTagged);

    if (!IsSupportedCakeVersion(BuildMetaData.CakeVersion, buildVersion.CakeVersion))
    {
        if (HasArgument("ignore-cake-version"))
        {
            Warning("Currently running Cake version {0}. This version is not supported together with Cake.Recipe.", buildVersion.CakeVersion);
            Warning("--ignore-cake-version Switch was found. Continuing execution of Cake.Recipe.");
        }
        else
        {
            throw new Exception(string.Format("Cake.Recipe currently only supports building projects using version {0} of Cake.  Please update your packages.config file (or whatever method is used to pin to a specific version of Cake) to use this version. Or use the --ignore-cake-version switch if you know what you are doing!", BuildMetaData.CakeVersion));
        }
    }

    return buildVersion;
});
#endif

Setup<BuildData>(context =>
{
    Information(Figlet(BuildParameters.Title));

    Information("Starting Setup...");

    if (BuildParameters.BranchType == BranchType.Master && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    // Make sure build and linters run before issues task.
    IssuesBuildTasks.ReadIssuesTask
        .IsDependentOn("Build")
        .IsDependentOn("InspectCode");

    return new BuildData(context);
});

Setup<DotNetCoreMSBuildSettings>(context =>
{
    var buildVersion = context.Data.Get<BuildVersion>();
    var data = context.Data.Get<BuildData>(); // Future use

    var settings = new DotNetCoreMSBuildSettings()
                    .WithProperty("Version", buildVersion.SemVersion)
                    .WithProperty("AssemblyVersion", buildVersion.Version)
                    .WithProperty("FileVersion", buildVersion.Version)
                    .WithProperty("AssemblyInformationalVersion", buildVersion.InformationalVersion);

    if (BuildParameters.ShouldUseDeterministicBuilds)
    {
        settings.WithProperty("ContinuousIntegrationBuild", "true");
    }
    if (BuildParameters.ShouldUseTargetFrameworkPath && BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
    {
        // This needs to be updated when/if we ever starts supporting Cake .NET Core edition.
        var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        context.Information("Will use FrameworkPathOverride={0} on .NET Core build related tasks since not building on Windows.", frameworkPathOverride);
        settings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }

    return settings;
});