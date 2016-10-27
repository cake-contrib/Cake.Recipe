public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(DirectoryPath sourceDirectoryPath,
        ICakeContext context
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        // Directories
        var buildDirectoryPath             = "./BuildArtifacts";
        var tempBuildDirectoryPath         = buildDirectoryPath + "/temp";
        var publishedNUnitTestsDirectory   = tempBuildDirectoryPath + "/_PublishedNUnitTests";
        var publishedxUnitTestsDirectory   = tempBuildDirectoryPath + "/_PublishedxUnitTests";
        var publishedMSTestTestsDirectory  = tempBuildDirectoryPath + "/_PublishedMSTestTests";
        var publishedWebsitesDirectory     = tempBuildDirectoryPath + "/_PublishedWebsites";
        var publishedApplicationsDirectory = tempBuildDirectoryPath + "/_PublishedApplications";
        var publishedLibrariesDirectory    = tempBuildDirectoryPath + "/_PublishedLibraries";

        var nugetNuspecDirectory = "./nuspec/nuget";
        var chocolateyNuspecDirectory = "./nuspec/chocolatey";

        var testResultsDirectory = buildDirectoryPath + "/TestResults";
        var inspectCodeResultsDirectory = testResultsDirectory + "/InspectCode";
        var dupFinderResultsDirectory = testResultsDirectory + "/DupFinder";
        var NUnitTestResultsDirectory = testResultsDirectory + "/NUnit";
        var xUnitTestResultsDirectory = testResultsDirectory + "/xUnit";
        var MSTestTestResultsDirectory = testResultsDirectory + "/MSTest";

        var testCoverageDirectory = buildDirectoryPath + "/TestCoverage";

        var nuGetPackagesOutputDirectory = buildDirectoryPath + "/Packages/NuGet";
        var chocolateyPackagesOutputDirectory = buildDirectoryPath + "/Packages/Chocolatey";

        // Files
        var testCoverageOutputFilePath = ((DirectoryPath)testCoverageDirectory).CombineWithFilePath("OpenCover.xml");
        var solutionInfoFilePath = ((DirectoryPath)sourceDirectoryPath).CombineWithFilePath("SolutionInfo.cs");

        var repoFilesPaths = new FilePath[] {
            "LICENSE",
            "README.md"
        };

        var buildDirectories = new BuildDirectories(
            sourceDirectoryPath,
            buildDirectoryPath,
            tempBuildDirectoryPath,
            publishedNUnitTestsDirectory,
            publishedxUnitTestsDirectory,
            publishedMSTestTestsDirectory,
            publishedWebsitesDirectory,
            publishedApplicationsDirectory,
            publishedLibrariesDirectory,
            nugetNuspecDirectory,
            chocolateyNuspecDirectory,
            testResultsDirectory,
            inspectCodeResultsDirectory,
            dupFinderResultsDirectory,
            NUnitTestResultsDirectory,
            xUnitTestResultsDirectory,
            MSTestTestResultsDirectory,
            testCoverageDirectory,
            nuGetPackagesOutputDirectory,
            chocolateyPackagesOutputDirectory
            );

        var buildFiles = new BuildFiles(
            context,
            repoFilesPaths,
            testCoverageOutputFilePath,
            solutionInfoFilePath
            );

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public ICollection<FilePath> RepoFilesPaths { get; private set; }

    public FilePath TestCoverageOutputFilePath { get; private set; }

    public FilePath SolutionInfoFilePath { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath[] repoFilesPaths,
        FilePath testCoverageOutputFilePath,
        FilePath solutionInfoFilePath
        )
    {
        RepoFilesPaths = Filter(context, repoFilesPaths);
        TestCoverageOutputFilePath = testCoverageOutputFilePath;
        SolutionInfoFilePath = solutionInfoFilePath;
    }

    private static FilePath[] Filter(ICakeContext context, FilePath[] files)
    {
        // Not a perfect solution, but we need to filter PDB files
        // when building on an OS that's not Windows (since they don't exist there).

        if(!context.IsRunningOnWindows())
        {
            return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
        }

        return files;
    }
}

public class BuildDirectories
{
    public DirectoryPath Source { get; private set; }
    public DirectoryPath Build { get; private set; }
    public DirectoryPath TempBuild { get; private set; }
    public DirectoryPath PublishedNUnitTests { get; private set; }
    public DirectoryPath PublishedxUnitTests { get; private set; }
    public DirectoryPath PublishedMSTestTests { get; private set; }
    public DirectoryPath PublishedWebsites { get; private set; }
    public DirectoryPath PublishedApplications { get; private set; }
    public DirectoryPath PublishedLibraries { get; private set; }
    public DirectoryPath NugetNuspecDirectory { get; private set; }
    public DirectoryPath ChocolateyNuspecDirectory { get; private set; }
    public DirectoryPath TestResults { get; private set; }
    public DirectoryPath InspectCodeTestResults { get; private set; }
    public DirectoryPath DupFinderTestResults { get; private set; }
    public DirectoryPath NUnitTestResults { get; private set; }
    public DirectoryPath xUnitTestResults { get; private set; }
    public DirectoryPath MSTestTestResults { get; private set; }
    public DirectoryPath TestCoverage { get; private set; }
    public DirectoryPath NuGetPackages { get; private set; }
    public DirectoryPath ChocolateyPackages { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath source,
        DirectoryPath build,
        DirectoryPath tempBuild,
        DirectoryPath publishedNUnitTests,
        DirectoryPath publishedxUnitTests,
        DirectoryPath publishedMSTestTests,
        DirectoryPath publishedWebsites,
        DirectoryPath publishedApplications,
        DirectoryPath publishedLibraries,
        DirectoryPath nugetNuspecDirectory,
        DirectoryPath chocolateyNuspecDirectory,
        DirectoryPath testResults,
        DirectoryPath inspectCodeTestResults,
        DirectoryPath dupFinderTestResults,
        DirectoryPath nunitTestResults,
        DirectoryPath xunitTestResults,
        DirectoryPath msTestTestResults,
        DirectoryPath testCoverage,
        DirectoryPath nuGetPackages,
        DirectoryPath chocolateyPackages
        )
    {
        Source = source;
        Build = build;
        TempBuild = tempBuild;
        PublishedNUnitTests = publishedNUnitTests;
        PublishedxUnitTests = publishedxUnitTests;
        PublishedMSTestTests = publishedMSTestTests;
        PublishedWebsites = publishedWebsites;
        PublishedApplications = publishedApplications;
        PublishedLibraries = publishedLibraries;
        NugetNuspecDirectory = nugetNuspecDirectory;
        ChocolateyNuspecDirectory = chocolateyNuspecDirectory;
        TestResults = testResults;
        InspectCodeTestResults = inspectCodeTestResults;
        DupFinderTestResults = dupFinderTestResults;
        NUnitTestResults = nunitTestResults;
        xUnitTestResults = xunitTestResults;
        MSTestTestResults = msTestTestResults;
        TestCoverage = testCoverage;
        NuGetPackages = nuGetPackages;
        ChocolateyPackages = chocolateyPackages;

        ToClean = new[] {
            Build,
            TempBuild,
            TestResults,
            TestCoverage
        };
    }
}