public static class ToolSettings
{
    public static string[] DupFinderExcludePattern { get; private set; }
    public static string[] DupFinderExcludeFilesByStartingCommentSubstring { get; private set; }
    public static int? DupFinderDiscardCost { get; private set; }
    public static bool? DupFinderThrowExceptionOnFindingDuplicates { get; private set; }
    public static string TestCoverageFilter { get; private set; }
    public static string TestCoverageExcludeByAttribute { get; private set; }
    public static string TestCoverageExcludeByFile { get; private set; }
    public static PlatformTarget BuildPlatformTarget { get; private set; }
    public static DirectoryPath OutputDirectory { get; private set; }

    public static void SetToolSettings(
        ICakeContext context,
        string[] dupFinderExcludePattern = null,
        string testCoverageFilter = null,
        string testCoverageExcludeByAttribute = null,
        string testCoverageExcludeByFile = null,
        PlatformTarget? buildPlatformTarget = null,
        DirectoryPath outputDirectory = null,
        string[] dupFinderExcludeFilesByStartingCommentSubstring = null,
        int? dupFinderDiscardCost = null,
        bool? dupFinderThrowExceptionOnFindingDuplicates = null
    )
    {
        context.Information("Setting up tools...");
        
        // TODO: Remove hard coding of Cake.Example.Tests
        DupFinderExcludePattern = dupFinderExcludePattern ?? new string[] { context.MakeAbsolute(context.Environment.WorkingDirectory) + "/src/Cake.Example.Tests/*.cs" };
        DupFinderExcludeFilesByStartingCommentSubstring = dupFinderExcludeFilesByStartingCommentSubstring;
        DupFinderDiscardCost = dupFinderDiscardCost;
        DupFinderThrowExceptionOnFindingDuplicates = dupFinderThrowExceptionOnFindingDuplicates;
        TestCoverageFilter = testCoverageFilter ?? "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]*";
        TestCoverageExcludeByAttribute = testCoverageExcludeByAttribute ?? "*.ExcludeFromCodeCoverage*";
        TestCoverageExcludeByFile = testCoverageExcludeByFile ?? "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs";
        BuildPlatformTarget = buildPlatformTarget ?? PlatformTarget.MSIL;
        OutputDirectory = outputDirectory;
    }
}