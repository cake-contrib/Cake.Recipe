[Flags]
public enum CoverletOutputFormat
{
    Cobertura = 1,
    JSON = 2,
    LCOV = 4,
    OpenCover = 8,
    TeamCity = 16,

    Deterministic = Cobertura | JSON,
    All = Cobertura | JSON | LCOV | OpenCover | TeamCity
}

public class CoverletSettings
{
    public CoverletOutputFormat Format { get; set; }

    public bool? UseSourceLink { get; set; }

    public bool UseDeterministicReport { get; set; }

    public List<string> ExcludeByFile { get; } = new List<string>();
    public List<string> ExcludeByAttribute { get; } = new List<string>();
    public List<string> Excludes { get; } = new List<string>();
    public List<string> Includes { get; } = new List<string>();
}

private class CoverletConsoleContext : CakeContextAdapter
{
    private readonly CoverletConsoleProcessRunner _runner;

    public override ICakeLog Log { get; }

    public override IProcessRunner ProcessRunner => _runner;

    public FilePath FilePath => _runner.FilePath;

    public ProcessSettings Settings => _runner.ProcessSettings;

    public CoverletConsoleContext(ICakeContext context)
        : base(context)
    {
        Log = new NullLog();
        _runner = new CoverletConsoleProcessRunner();
    }
}

private class CoverletConsoleProcessRunner : IProcessRunner
{
    public FilePath FilePath { get; set; }

    public ProcessSettings ProcessSettings { get; set; }

    private sealed class InterceptedProcess : IProcess
    {
        public void Dispose()
        {
        }

        public void WaitForExit()
        {
        }

        public bool WaitForExit(int milliseconds)
        {
            return true;
        }

        public int GetExitCode()
        {
            return 0;
        }

        public IEnumerable<string> GetStandardError()
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetStandardOutput()
        {
            return Enumerable.Empty<string>();
        }

        public void Kill()
        {
        }
    }

    public IProcess Start(FilePath filePath, ProcessSettings settings)
    {
        FilePath = filePath;
        ProcessSettings = settings;
        return new InterceptedProcess();
    }
}

public void RunCoverletConsole(ICakeContext context, Action<ICakeContext, FilePath> action, FilePathCollection files)
    => RequireTool(ToolSettings.CoverletGlobalTool, () =>
{
    var tool = BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows
        ? context.Tools.Resolve("coverlet.exe")
        : context.Tools.Resolve("coverlet");

    if (tool is null)
    {
        throw new CakeException("Coverlet tool was not found.");
    }

    var settings = ToolSettings.Coverlet ?? new CoverletSettings();

    if (settings.UseDeterministicReport)
    {
        context.Warning("Deterministic Report is not supported when running Coverlet Console. Disabling deterministic report!");
        settings.UseDeterministicReport = false;
    }

    UpdateCoverletFilters(settings);

    var outputDir = BuildParameters.Paths.Directories.TestCoverage.Combine("coverlet");

    foreach (var file in files)
    {
        var absoluteFilePath = file.MakeAbsolute(context.Environment);
        var interceptor = new CoverletConsoleContext(context);
        action(interceptor, file);

        var arguments = new ProcessSettings
        {
            Arguments = absoluteFilePath.FullPath.Quote()
        };

        arguments.Arguments.AppendSwitchQuoted("--target", interceptor.FilePath.MakeAbsolute(context.Environment).FullPath);
        var renderedArguments = interceptor.Settings.Arguments?.Render();

        if (!string.IsNullOrEmpty(renderedArguments))
        {
            renderedArguments = renderedArguments.Replace("\"", "\\\"");
            arguments.Arguments.AppendSwitchQuoted("--targetargs", renderedArguments);
        }

        arguments.Arguments.AppendSwitchQuoted("--output", outputDir.Combine(Guid.NewGuid().ToString()) + "/");

        foreach (var format in GetFormatList(context, settings))
        {
            arguments.Arguments.AppendSwitchQuoted("--format", format);
        }

        if (settings.UseSourceLink.GetValueOrDefault())
        {
            arguments.Arguments.Append("--use-source-link");
        }

        var exitCode = context.StartProcess(tool, arguments);

        if (exitCode != 0)
        {
            throw new CakeException(exitCode);
        }
    }
});

// <returns>An enumerable of project paths that was not handled by coverlet</returns>
public static IEnumerable<string> RunCoverlet(ICakeContext context, DotNetCoreMSBuildSettings mSBuildSettings)
{
    var settings = ToolSettings.Coverlet ?? new CoverletSettings();

    UpdateCoverletFilters(settings);

    var isAuto = ToolSettings.CoverageTool == CoverageToolType.Auto || ToolSettings.CoverageTool == CoverageToolType.CoverletAuto;

    if (isAuto || ToolSettings.CoverageTool == CoverageToolType.CoverletMSBuild)
    {
        var determineSourceLink = isAuto && settings.UseSourceLink is null;

        var projects = context.GetFiles(BuildParameters.TestDirectoryPath + (BuildParameters.TestFilePattern ?? "/**/*Tests.csproj"));
        context.Information("Found {0} test projects", projects.Count);

        foreach (var project in projects)
        {
            var parsedProject = context.ParseProject(project, BuildParameters.Configuration);

            if (determineSourceLink)
            {
                settings.UseSourceLink = HasSourceLinkPackage(parsedProject);
            }

            if (isAuto && parsedProject.HasPackage("coverlet.collector"))
            {
                RunCoverletCollector(context, project.FullPath, mSBuildSettings, settings);
            }
            else if (!isAuto || parsedProject.HasPackage("coverlet.msbuild"))
            {
                context.Warning("Using legacy Coverlet MSBuild for {0}. Due to problems on Linux it is recommended to switch to coverlet.collector instead!", project);
                var name = parsedProject.RootNameSpace.Replace('.', '-');
                var outputDirectory = context.MakeAbsolute(BuildParameters.Paths.Directories.TestCoverage.Combine("coverlet").Combine(name)).FullPath;
                RunCoverletDotNetMsBuild(context, project.FullPath, mSBuildSettings, settings, outputDirectory);
            }
            else if (ToolSettings.CoverageTool == CoverageToolType.CoverletAuto)
            {
                context.Warning("Unable to run Coverlet for project. Please add a reference to coverlet.collector or coverlet.msbuild.");

                var dotNetSettings = new DotNetCoreTestSettings
                {
                    Configuration = BuildParameters.Configuration,
                    NoBuild       = true,
                };
                context.DotNetCoreTest(project.FullPath, dotNetSettings);
            }
            else
            {
                // No supported package was found, as such we
                // return the path of the project so it can run
                // through other processes.
                yield return project.FullPath;
            }
        }
    }
    else if (ToolSettings.CoverageTool == CoverageToolType.CoverletCollector)
    {
        RunCoverletCollector(context, BuildParameters.SolutionFilePath.FullPath, mSBuildSettings, settings);
    }
    else
    {
        // We return string.Empty to indicate that no projects
        // was attempted.
        yield return string.Empty;
    }
}

public static void RunCoverletDotNetMsBuild(ICakeContext context, string solutionOrProject, DotNetCoreMSBuildSettings msBuildSettings, CoverletSettings settings, string outputDirectory)
{
    context.EnsureDirectoryExists(outputDirectory);

    if (!outputDirectory.EndsWith("/") && !outputDirectory.EndsWith("\\"))
    {
        outputDirectory = outputDirectory + "/";
    }

    var dotNetSettings = new DotNetCoreTestSettings
    {
        Configuration = BuildParameters.Configuration,
        NoBuild       = true,
        ArgumentCustomization = args => {
            args.AppendMSBuildSettings(msBuildSettings, context.Environment);

            args.AppendSwitch("/p:CollectCoverage", "=", "true");

            var formats = GetFormat(context, settings);
            args.AppendSwitch("/p:CoverletOutputFormat", "=", "\\\"" + formats + "\\\"");
            args.AppendSwitchQuoted("/p:CoverletOutput", "=", outputDirectory);

            if (settings.UseSourceLink.GetValueOrDefault())
            {
                args.AppendSwitch("/p:UseSourceLink", "=", "true");
            }

            if (settings.UseDeterministicReport)
            {
                args.AppendSwitch("/p:DeterministicReport", "=", "true");
            }

            if (settings.ExcludeByFile.Count > 0)
            {
                args.AppendSwitch("/p:ExcludeByFile", "=", "\\\"" + string.Join(",", settings.ExcludeByFile) + "\\\"");
            }

            if (settings.ExcludeByAttribute.Count > 0)
            {
                args.AppendSwitch("/p:ExcludeByAttribute", "=", "\\\"" + string.Join(",", settings.ExcludeByAttribute) + "\\\"");
            }

            if (settings.Includes.Count > 0)
            {
                args.AppendSwitch("/p:Include", "=", "\\\"" + string.Join(",", settings.Includes) + "\\\"");
            }

            if (settings.Excludes.Count > 0)
            {
                args.AppendSwitchQuoted("/p:Exclude", "=", "\\\"" + string.Join(",", settings.Excludes) + "\\\"");
            }

            return args;
        }
    };

    context.DotNetCoreTest(solutionOrProject, dotNetSettings);
}

public static void RunCoverletCollector(ICakeContext context, string solutionOrProject, DotNetCoreMSBuildSettings msBuildSettings, CoverletSettings settings)
{
    var collectorSb = new StringBuilder("XPlat Code Coverage");

    var formats = GetFormat(context, settings);
    collectorSb.Append(";Format=").Append(formats);

    if (settings.UseSourceLink.GetValueOrDefault())
    {
        collectorSb.Append(";UseSourceLink=True");
    }

    if (settings.UseDeterministicReport)
    {
        collectorSb.Append(";DeterministicReport=True");
    }

    if (settings.ExcludeByFile.Count > 0)
    {
        collectorSb.Append(";ExcludeByFile=").Append(string.Join(",", settings.ExcludeByFile));
    }

    if (settings.ExcludeByAttribute.Count > 0)
    {
        collectorSb.Append(";ExcludeByAttribute=").Append(string.Join(",", settings.ExcludeByAttribute));
    }

    if (settings.Includes.Count > 0)
    {
        collectorSb.Append(";Include=").Append(string.Join(",", settings.Includes));
    }

    if (settings.Excludes.Count > 0)
    {
        collectorSb.Append(";Exclude=").Append(string.Join(",", settings.Excludes));
    }

    var dotNetSettings = new DotNetCoreTestSettings
    {
        Configuration = BuildParameters.Configuration,
        NoBuild       = true,
        Collectors    = new[]{ collectorSb.ToString() },
        ArgumentCustomization = args => {
            args.AppendMSBuildSettings(msBuildSettings, context.Environment);
            return args;
        },
        ResultsDirectory = BuildParameters.Paths.Directories.TestCoverage.Combine("coverlet")
    };

    context.DotNetCoreTest(BuildParameters.SolutionFilePath.FullPath, dotNetSettings);
}

private static bool HasSourceLinkPackage(CustomProjectParserResult project)
{
    return
        project.HasPackage("Microsoft.SourceLink.GitHub") ||
        project.HasPackage("Microsoft.SourceLink.AzureRepos.Git") ||
        project.HasPackage("Microsoft.SourceLink.GitLab") ||
        project.HasPackage("Microsoft.SourceLink.BitBucket.Git") ||
        project.HasPackage("Microsoft.SourceLink.AzureDevOpsServer.Git") ||
        project.HasPackage("Microsoft.SourceLink.Gitea") ||
        project.HasPackage("Microsoft.SourceLink.GitWeb");
}

private static void UpdateCoverletFilters(CoverletSettings settings)
{
    settings.ExcludeByFile.AddRange(ToolSettings.TestCoverageExcludeByFile.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries));
    settings.ExcludeByAttribute.AddRange(ToolSettings.TestCoverageExcludeByAttribute.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries));

    foreach (var filter in ToolSettings.TestCoverageFilter.Split(new[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()))
    {
        if (filter.Length == 0)
        {
            continue;
        }

        if (filter[0] == '+')
        {
            settings.Includes.Add(filter.Substring(1));
        }
        else if (filter[0] == '-')
        {
            settings.Excludes.Add(filter.Substring(1));
        }
        else
        {
            settings.Includes.Add(filter);
        }
    }
}

private static List<string> GetFormatList(ICakeContext context, CoverletSettings settings)
{
    var results = new List<string>();

    ReadOnlySpan<CoverletOutputFormat> unsupportedDeterministicFormats = stackalloc CoverletOutputFormat[]
    {
        CoverletOutputFormat.OpenCover,
        CoverletOutputFormat.LCOV,
        CoverletOutputFormat.TeamCity
    };

    foreach (var availableFormat in Enum.GetValues(typeof(CoverletOutputFormat)).Cast<CoverletOutputFormat>())
    {
        if (availableFormat == CoverletOutputFormat.All || availableFormat == CoverletOutputFormat.Deterministic)
        {
            continue;
        }

        if (settings.Format.HasFlag(availableFormat))
        {
            if (settings.UseDeterministicReport && HasFormat(unsupportedDeterministicFormats, availableFormat))
            {
                context.Warning("Deterministic report is not supported when using {0} format. We will not be adding the {0} format!", availableFormat);
                continue;
            }

            results.Add(availableFormat.ToString().ToLowerInvariant());
        }
    }

    if (results.Count == 0)
    {
        if (settings.UseDeterministicReport)
        {
            results.Add("cobertura");
        }
        else
        {
            results.Add("opencover");
        }
    }

    return results;
}

private static string GetFormat(ICakeContext context, CoverletSettings settings)
{
    return string.Join(",", GetFormatList(context, settings));
}

private static bool HasFormat(ReadOnlySpan<CoverletOutputFormat> formats, CoverletOutputFormat testFormat)
{
    for (int i = 0; i < formats.Length; i++)
    {
        if (formats[i] == testFormat)
        {
            return true;
        }
    }

    return false;
}
