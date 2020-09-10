public static class ToolSettings
{
    static ToolSettings()
    {
        SetToolPreprocessorDirectives();
    }

    public static string[] DupFinderExcludePattern { get; private set; }
    public static string[] DupFinderExcludeFilesByStartingCommentSubstring { get; private set; }
    public static int? DupFinderDiscardCost { get; private set; }
    public static bool? DupFinderThrowExceptionOnFindingDuplicates { get; private set; }
    public static string TestCoverageFilter { get; private set; }
    public static string TestCoverageExcludeByAttribute { get; private set; }
    public static string TestCoverageExcludeByFile { get; private set; }
    public static PlatformTarget BuildPlatformTarget { get; private set; }
    public static MSBuildToolVersion BuildMSBuildToolVersion { get; private set; }
    public static int MaxCpuCount { get; private set; }
    public static string TargetFrameworkPathOverride { get; private set; }

    public static string CodecovTool { get; private set; }
    public static string CoverallsTool { get; private set; }
    public static string GitReleaseManagerTool { get; private set; }
    public static string GitVersionTool { get; private set; }
    public static string ReSharperTools { get; private set; }
    public static string KuduSyncTool { get; private set; }
    public static string WyamTool { get; private set; }
    public static string XUnitTool { get; private set; }
    public static string NUnitTool { get; private set; }
    public static string NuGetTool { get; private set; }
    public static string OpenCoverTool { get; private set; }
    public static string ReportGeneratorTool { get; private set; }
    public static string ReportUnitTool { get; private set; }

    public static string CodecovGlobalTool { get; private set; }
    public static string CoverallsGlobalTool { get; private set; }
    public static string GitReleaseManagerGlobalTool { get; private set; }
    public static string GitVersionGlobalTool { get; private set; }
    public static string ReportGeneratorGlobalTool { get; private set; }
    public static string WyamGlobalTool { get; private set; }
    public static string KuduSyncGlobalTool { get; private set; }

    public static void SetToolPreprocessorDirectives(
        string codecovTool = "#tool nuget:?package=codecov&version=1.12.3",
        // This is specifically pinned to 0.7.0 as later versions of same package publish .Net Global Tool, rather than full framework version
        string coverallsTool = "#tool nuget:?package=coveralls.net&version=0.7.0",
        string gitReleaseManagerTool = "#tool nuget:?package=GitReleaseManager&version=0.11.0",
        // This is specifically pinned to 5.0.1 as later versions break compatibility with Unix.
        string gitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=5.0.1",
        string reSharperTools = "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2020.2.2",
        string kuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.5.3",
        string wyamTool = "#tool nuget:?package=Wyam&version=2.2.9",
        string xunitTool = "#tool nuget:?package=xunit.runner.console&version=2.4.1",
        string nunitTool = "#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1",
        string nugetTool = "#tool nuget:?package=NuGet.CommandLine&version=5.7.0",
        string openCoverTool = "#tool nuget:?package=OpenCover&version=4.7.922",
        string reportGeneratorTool = "#tool nuget:?package=ReportGenerator&version=4.6.6",
        string reportUnitTool = "#tool nuget:?package=ReportUnit&version=1.2.1",
        string codecovGlobalTool = "#tool dotnet:?package=Codecov.Tool&version=1.12.3",
        string coverallsGlobalTool = "#tool dotnet:?package=coveralls.net&version=1.0.0",
        string gitReleaseManagerGlobalTool = "#tool dotnet:?package=GitReleaseManager.Tool&version=0.11.0",
        string gitVersionGlobalTool = "#tool dotnet:?package=GitVersion.Tool&version=5.3.7",
        string reportGeneratorGlobalTool = "#tool dotnet:?package=dotnet-reportgenerator-globaltool&version=4.6.6",
        string wyamGlobalTool = "#tool dotnet:?package=Wyam.Tool&version=2.2.9",
        // This is using an unofficial build of kudusync so that we can have a .Net Global tool version.  This was generated from this PR: https://github.com/projectkudu/KuduSync.NET/pull/27
        string kuduSyncGlobalTool = "#tool dotnet:https://www.myget.org/F/cake-contrib/api/v3/index.json?package=KuduSync.Tool&version=1.5.4-gc5cc5a2a19"
    )
    {
        CodecovTool = codecovTool;
        CoverallsTool = coverallsTool;
        GitReleaseManagerTool = gitReleaseManagerTool;
        GitVersionTool = gitVersionTool;
        ReSharperTools = reSharperTools;
        KuduSyncTool = kuduSyncTool;
        WyamTool = wyamTool;
        XUnitTool = xunitTool;
        NUnitTool = nunitTool;
        NuGetTool = nugetTool;
        OpenCoverTool = openCoverTool;
        ReportGeneratorTool = reportGeneratorTool;
        ReportUnitTool = reportUnitTool;
        ReportGeneratorGlobalTool = reportGeneratorGlobalTool;
        GitVersionGlobalTool = gitVersionGlobalTool;
        GitReleaseManagerGlobalTool = gitReleaseManagerGlobalTool;
        CodecovGlobalTool = codecovGlobalTool;
        CoverallsGlobalTool = coverallsGlobalTool;
        WyamGlobalTool = wyamGlobalTool;
        KuduSyncGlobalTool = kuduSyncGlobalTool;
    }

    public static void SetToolSettings(
        ICakeContext context,
        string[] dupFinderExcludePattern = null,
        string testCoverageFilter = null,
        string testCoverageExcludeByAttribute = null,
        string testCoverageExcludeByFile = null,
        PlatformTarget? buildPlatformTarget = null,
        MSBuildToolVersion buildMSBuildToolVersion = MSBuildToolVersion.Default,
        int? maxCpuCount = null,
        DirectoryPath targetFrameworkPathOverride = null,
        string[] dupFinderExcludeFilesByStartingCommentSubstring = null,
        int? dupFinderDiscardCost = null,
        bool? dupFinderThrowExceptionOnFindingDuplicates = null
    )
    {
        context.Information("Setting up tools...");

        var absoluteTestDirectory = context.MakeAbsolute(BuildParameters.TestDirectoryPath);
        var absoluteSourceDirectory = context.MakeAbsolute(BuildParameters.SolutionDirectoryPath);
        DupFinderExcludePattern = dupFinderExcludePattern ??
            new string[]
            {
                string.Format("{0}/{1}.Tests/**/*.cs", absoluteTestDirectory, BuildParameters.Title),
                string.Format("{0}/**/*.AssemblyInfo.cs", absoluteSourceDirectory)
            };
        DupFinderExcludeFilesByStartingCommentSubstring = dupFinderExcludeFilesByStartingCommentSubstring;
        DupFinderDiscardCost = dupFinderDiscardCost;
        DupFinderThrowExceptionOnFindingDuplicates = dupFinderThrowExceptionOnFindingDuplicates;
        TestCoverageFilter = testCoverageFilter ?? string.Format("+[{0}*]* -[*.Tests]*", BuildParameters.Title);
        TestCoverageExcludeByAttribute = testCoverageExcludeByAttribute ?? "*.ExcludeFromCodeCoverage*";
        TestCoverageExcludeByFile = testCoverageExcludeByFile ?? "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs";
        BuildPlatformTarget = buildPlatformTarget ?? PlatformTarget.MSIL;
        BuildMSBuildToolVersion = buildMSBuildToolVersion;
        MaxCpuCount = maxCpuCount ?? 0;
        if (BuildParameters.ShouldUseTargetFrameworkPath && targetFrameworkPathOverride == null)
        {
            if (context.Environment.Runtime.IsCoreClr)
            {
                var path = context.Tools.Resolve("mono").GetDirectory();
                path = path.Combine("../lib/mono/4.5");
                TargetFrameworkPathOverride = path.FullPath + "/";
            }
            else
            {
                TargetFrameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";
            }
        }
        else
        {
            TargetFrameworkPathOverride = targetFrameworkPathOverride?.FullPath;
        }
    }
}
