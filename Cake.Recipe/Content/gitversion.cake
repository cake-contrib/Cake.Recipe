public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string Milestone { get; private set; }
    public string CakeVersion { get; private set; }
    public string InformationalVersion { get; private set; }
    public string FullSemVersion { get; private set; }
    public string AssemblySemVer { get; private set; }

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
        GitVersion assertedVersions = null;

        if (BuildParameters.ShouldCalculateVersion)
        {
            if (BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows) {
                PatchGitLibConfigFiles(context);
            }

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
            SemVersion = semVersion?.ToLowerInvariant(),
            Milestone = BuildParameters.IsTagged || context.HasArgument("create-pre-release") ? milestone : version,
            CakeVersion = cakeVersion,
            InformationalVersion = informationalVersion?.ToLowerInvariant(),
            FullSemVersion = fullSemVersion?.ToLowerInvariant(),
            AssemblySemVer = assemblySemVer?.ToLowerInvariant()
        };
    }

    private static void PatchGitLibConfigFiles(ICakeContext context)
    {
        var configFiles = context.GetFiles("./tools/**/LibGit2Sharp.dll.config");
        var libgitPath = GetLibGit2Path(context);
        if (string.IsNullOrEmpty(libgitPath)) { return; }

        foreach (var config in configFiles) {
            var xml = System.Xml.Linq.XDocument.Load(config.ToString());

            if (xml.Element("configuration").Elements("dllmap")
                .All(e => e.Attribute("target").Value != libgitPath)) {

                var dllName = xml.Element("configuration").Elements("dllmap").First(e => e.Attribute("os").Value == "linux").Attribute("dll").Value;
                xml.Element("configuration")
                    .Add(new System.Xml.Linq.XElement("dllmap",
                        new System.Xml.Linq.XAttribute("os", "linux"),
                        new System.Xml.Linq.XAttribute("dll", dllName),
                        new System.Xml.Linq.XAttribute("target", libgitPath)));

                context.Information($"Patching '{config}' to use fallback system path on Linux...");
                xml.Save(config.ToString());
            }
        }
    }

    private static string GetLibGit2Path(ICakeContext context)
    {
        var possiblePaths = new[] {
            "/usr/lib*/libgit2.so*",
            "/usr/lib/*/libgit2.so*"
        };

        foreach (var path in possiblePaths) {
            var file = context.GetFiles(path).FirstOrDefault();
            if (file != null && !string.IsNullOrEmpty(file.ToString())) {
                return file.ToString();
            }
        }

        return null;
    }
}
