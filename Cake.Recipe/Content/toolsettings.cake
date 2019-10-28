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
    public static DirectoryPath OutputDirectory { get; private set; }

    public static string CodecovTool { get; private set; }
    public static string CoverallsTool { get; private set; }
    public static string GitReleaseManagerTool { get; private set; }
    public static string GitVersionTool { get; private set; }
    public static string ReSharperTools { get; private set; }
    public static string ReSharperReportsTool { get; private set; }
    public static string KuduSyncTool { get; private set; }
    public static string WyamTool { get; private set; }
    public static string MSBuildExtensionPackTool { get; private set; }
    public static string XUnitTool { get; private set; }
    public static string NUnitTool { get; private set; }
    public static string OpenCoverTool { get; private set; }
    public static string ReportGeneratorTool { get; private set; }
    public static string ReportUnitTool { get; private set; }
    public static string FixieTool { get; private set; }

    public static void SetToolPreprocessorDirectives( 
        string codecovTool = "#tool nuget:?package=codecov&version=1.7.2",
        string coverallsTool = "#tool nuget:?package=coveralls.io&version=1.4.2",
        string gitReleaseManagerTool = "#tool nuget:?package=GitReleaseManager&version=0.8.0",
        string gitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=5.0.1",
        string reSharperTools = "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2019.2.3",
        string reSharperReportsTool = "#tool nuget:?package=ReSharperReports&version=0.4.0",
        string kuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.5.2",
        string wyamTool = "#tool nuget:?package=Wyam&version=2.2.9",
        string msbuildExtensionPackTool = "#tool nuget:?package=MSBuild.Extension.Pack&version=1.9.1",
        string xunitTool = "#tool nuget:?package=xunit.runner.console&version=2.4.1",
        string nunitTool = "#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0",
        string openCoverTool = "#tool nuget:?package=OpenCover&version=4.7.922",
        string reportGeneratorTool = "#tool nuget:?package=ReportGenerator&version=4.3.3",
        string reportUnitTool = "#tool nuget:?package=ReportUnit&version=1.2.1",
        string fixieTool = "#tool nuget:?package=Fixie&version=2.2.0"
    )
    {
        CodecovTool = codecovTool;
        CoverallsTool = coverallsTool;
        GitReleaseManagerTool = gitReleaseManagerTool;
        GitVersionTool = gitVersionTool;
        ReSharperTools = reSharperTools;
        ReSharperReportsTool = reSharperReportsTool;
        KuduSyncTool = kuduSyncTool;
        WyamTool = wyamTool;
        MSBuildExtensionPackTool = msbuildExtensionPackTool;
        XUnitTool = xunitTool;
        NUnitTool = nunitTool;
        OpenCoverTool = openCoverTool;
        ReportGeneratorTool = reportGeneratorTool;
        ReportUnitTool = reportUnitTool;
        FixieTool = fixieTool;
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
        DirectoryPath outputDirectory = null,
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
        OutputDirectory = outputDirectory;
    }
}
