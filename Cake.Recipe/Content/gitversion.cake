public class BuildVersion
{
    public string Version { get; set; }
    public string SemVersion { get; set; }
    public string Milestone { get; set; }
    public string CakeVersion { get; set; }
    public string InformationalVersion { get; set; }
    public string FullSemVersion { get; set; }
    public string AssemblySemVer { get; set; }

    public static BuildVersion CalculatingSemanticVersion(
        ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

        try
        {
            context.Information("Testing to see if valid git repository...");

            var rootPath = BuildParameters.RootDirectoryPath;
            rootPath = context.GitFindRootFromPath(rootPath);
        }
        catch (LibGit2Sharp.RepositoryNotFoundException)
        {
            context.Warning("Unable to locate git repository, so GitVersion can't be executed, returning default version numbers...");

            return new BuildVersion
            {
                Version = "0.1.0",
                SemVersion = "0.1.0-alpha.0",
                Milestone = "0.1.0",
                CakeVersion = cakeVersion,
                InformationalVersion = "0.1.0-alpha.0+Branch.develop.Sha.528f9bf572a52f0660cbe3f4d109599eab1e9866",
                FullSemVersion = "0.1.0-alpha.0",
                AssemblySemVer = "0.1.0.o"
            };
        }

        string version = null;
        string semVersion = null;
        string milestone = null;
        string informationalVersion = null;
        string assemblySemVer = null;
        string fullSemVersion = null;
        string uniqueSemVersion = null;
        GitVersion assertedVersions = null;

        if (BuildParameters.ShouldCalculateVersion)
        {
            BuildParameters.Platform.PatchGitLib2ConfigFiles("**/GitVersion*/**/LibGit2Sharp.dll.config");

            context.Information("Calculating Semantic Version...");
            if (!BuildParameters.IsLocalBuild || BuildParameters.IsPublishBuild || BuildParameters.IsReleaseBuild || BuildParameters.PrepareLocalRelease)
            {
                if (!BuildParameters.IsPublicRepository && BuildParameters.IsRunningOnAppVeyor)
                {
                    context.GitVersion(new GitVersionSettings{
                        UpdateAssemblyInfoFilePath = BuildParameters.Paths.Files.SolutionInfoFilePath,
                        UpdateAssemblyInfo = true,
                        OutputType = GitVersionOutput.BuildServer,
                        NoFetch = true
                    });
                } else {
                    context.GitVersion(new GitVersionSettings{
                        UpdateAssemblyInfoFilePath = BuildParameters.Paths.Files.SolutionInfoFilePath,
                        UpdateAssemblyInfo = true,
                        OutputType = GitVersionOutput.BuildServer
                    });
                }

                version = context.EnvironmentVariable("GitVersion_MajorMinorPatch");
                semVersion = context.EnvironmentVariable("GitVersion_LegacySemVerPadded");
                informationalVersion = context.EnvironmentVariable("GitVersion_InformationalVersion");
                assemblySemVer = context.EnvironmentVariable("GitVersion_AssemblySemVer");
                milestone = string.Concat(version);
                fullSemVersion = context.EnvironmentVariable("GitVersion_FullSemVer");
                uniqueSemVersion = string.Concat(context.EnvironmentVariable("GitVersion_LegacySemVerPadded"), "-", context.EnvironmentVariable("GitVersion_CommitsSinceVersionSourcePadded"));
            }

            if (!BuildParameters.IsPublicRepository && BuildParameters.IsRunningOnAppVeyor)
            {
                assertedVersions = context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.Json,
                        NoFetch = true
                });
            } else {
                assertedVersions = context.GitVersion(new GitVersionSettings{
                        OutputType = GitVersionOutput.Json,
                });
            }

            version = assertedVersions.MajorMinorPatch;
            semVersion = assertedVersions.LegacySemVerPadded;
            informationalVersion = assertedVersions.InformationalVersion;
            assemblySemVer = assertedVersions.AssemblySemVer;
            milestone = assertedVersions.SemVer;
            fullSemVersion = assertedVersions.FullSemVer;
            uniqueSemVersion = string.Concat(assertedVersions.LegacySemVerPadded, "-", assertedVersions.CommitsSinceVersionSourcePadded);

            context.Information("Calculated Semantic Version: {0}", semVersion);
        }

        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(semVersion))
        {
            context.Information("Fetching version from SolutionInfo...");
            var assemblyInfo = context.ParseAssemblyInfo(BuildParameters.Paths.Files.SolutionInfoFilePath);
            version = assemblyInfo.AssemblyVersion;
            semVersion = assemblyInfo.AssemblyInformationalVersion;
            informationalVersion = assemblyInfo.AssemblyInformationalVersion;
            milestone = string.Concat(version);
        }

        return new BuildVersion
        {
            Version = version,
            SemVersion = ((BuildParameters.BranchType == BranchType.HotFix || BuildParameters.BranchType == BranchType.Release) && !BuildParameters.IsTagged) ? uniqueSemVersion?.ToLowerInvariant() : semVersion?.ToLowerInvariant(),
            Milestone = BuildParameters.IsTagged || context.HasArgument("create-pre-release") ? milestone : version,
            CakeVersion = cakeVersion,
            InformationalVersion = informationalVersion?.ToLowerInvariant(),
            FullSemVersion = fullSemVersion?.ToLowerInvariant(),
            AssemblySemVer = assemblySemVer?.ToLowerInvariant()
        };
    }
}
