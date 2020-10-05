using System.Runtime.InteropServices;

public class BuildPlatform
{
    protected ICakeContext context;

    public BuildPlatform(ICakeContext context)
    {
        this.context = context;
        SystemVersion = System.Environment.OSVersion.Version.ToString();
        SystemName = RuntimeInformation.OSDescription.Trim();
        OperatingSystem = context.Environment.Platform.Family;
        SystemArchitecture = RuntimeInformation.OSArchitecture.ToString();
    }

    public PlatformFamily OperatingSystem { get; private set; }
    public string SystemName { get; private set; }
    public string SystemDistro { get; protected set; } = "";
    public string SystemDistroId { get; protected set; } = "";
    public string SystemVersion { get; protected set; }
    public string SystemArchitecture { get; private set; }

    public static BuildPlatform Create(ICakeContext context)
    {
        BuildPlatform result;
        if (context.Environment.Platform.Family != PlatformFamily.Linux)
        {
            result = new BuildPlatform(context);
        }
        else
        {
            result = new LinuxBuildPlatform(context);
        }

        return result;
    }

    public virtual void CopyLibGit2Binaries(params string[] globbingPatterns)
    {
    }

    public virtual void PatchGitLib2ConfigFiles(string globbingPattern = "**/LibGit2Sharp.dll.config")
    {
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(SystemDistro))
        {
            return string.Format("{0} {1} ({2})", SystemDistro, SystemVersion, SystemArchitecture);
        }
        else if (OperatingSystem == PlatformFamily.Windows)
        {
            // Windows have the system version in the SystemName
            return string.Format("{0} ({1})", SystemName, SystemArchitecture);
        }
        else
        {
            return string.Format("{0} {1} ({2})", SystemName, SystemVersion, SystemArchitecture);
        }
    }
}

public class LinuxBuildPlatform : BuildPlatform
{
    public LinuxBuildPlatform(ICakeContext context)
        : base(context)
    {
        SetSystemInformation();
    }

    public override void CopyLibGit2Binaries(params string[] globbingPatterns)
    {
        var addinPath = GetAddinPath(context);
        var copyToFiles = context.GetFiles(globbingPatterns.Select(g => addinPath.CombineWithFilePath(g).ToString()).ToArray());

        string libgit2Path = GetLibGit2Path(context);

        if (String.IsNullOrEmpty(libgit2Path))
        {
            return;
        }

        foreach (var file in copyToFiles)
        {
            if (context.FileExists(file))
            {
                context.DeleteFile(file);
            }
            context.Verbose("Copying system library from '{0}' to '{1}'", libgit2Path, file);
            context.CopyFile(libgit2Path, file);
        }
    }

    public override void PatchGitLib2ConfigFiles(string globbingPattern = "**/LibGit2Sharp.dll.config")
    {
        var toolPath = context.Configuration.GetToolPath(".", context.Environment);
        var configFiles = context.GetFiles(toolPath.CombineWithFilePath(globbingPattern).ToString());

        var libgit2Path = GetLibGit2Path(context);

        if (String.IsNullOrEmpty(libgit2Path))
        {
            return;
        }

        // This is a workaround for Ubuntu 18.04 especially.
        // as gitversion will fail if we add a dllmap to the system
        // libgit2 library.
        var relativeExpected = "lib/";
        if (!string.IsNullOrEmpty(SystemDistroId))
        {
            if (!string.IsNullOrEmpty(SystemVersion)
                && Version.TryParse(SystemVersion, out _))
            {
                relativeExpected += string.Format("{0}.{1}-{2}",
                    SystemDistroId,
                    SystemVersion,
                    SystemArchitecture);
            }
            else
            {
                relativeExpected += string.Format("{0}-{1}",
                    SystemDistroId,
                    SystemArchitecture);
            }
        }

        foreach (var config in configFiles)
        {
            if (!relativeExpected.Equals("lib/"))
            {
                var parent = config.GetDirectory();
                var expectedPath = parent.Combine(relativeExpected);
                if (context.DirectoryExists(expectedPath))
                {
                    continue;
                }
            }

            var xml = System.Xml.Linq.XDocument.Load(config.ToString());

            if (xml.Element("configuration").Elements("dllmap").All(e => e.Attribute("target").Value != libgit2Path))
            {
                var dllName = xml.Element("configuration").Elements("dllmap").First(e => e.Attribute("os").Value == "linux").Attribute("dll").Value;
                xml.Element("configuration")
                    .Add(new System.Xml.Linq.XElement("dllmap",
                        new System.Xml.Linq.XAttribute("os", "linux"),
                        new System.Xml.Linq.XAttribute("dll", dllName),
                        new System.Xml.Linq.XAttribute("target", libgit2Path)));

                context.Information($"Patching '{config}' to use fallback system path on Linux...");
                xml.Save(config.ToString());
            }
        }
    }

    // Shamelessly stolen from <https://github.com/cake-build/cake/blob/142cd24779129925e4e09a2d17a6a481782f31dc/src/Cake.Core/Scripting/ScriptRunner.cs#L190-L199>
    private static DirectoryPath GetAddinPath(ICakeContext context)
    {
        var addinPath = context.Configuration.GetValue("Paths_Addins");
        if (!string.IsNullOrEmpty(addinPath))
        {
            return new DirectoryPath(addinPath).MakeAbsolute(context.Environment);
        }

        var toolPath = context.Configuration.GetToolPath(".", context.Environment);
        return toolPath.Combine("Addins").Collapse();
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

    private void SetSystemInformation()
    {
        string file;
        if (context.FileExists("/etc/lsb-release"))
        {
            file = "/etc/lsb-release";
        }
        else if (context.FileExists("/etc/os-release"))
        {
            file = "/etc/os-release";
        }
        else
        {
            return;
        }

        var lines = System.IO.File.ReadAllLines(file);
        var details = lines.Where(l => !string.IsNullOrEmpty(l))
            .Select(l => l.Split(new [] {'=' }, StringSplitOptions.None))
            .Where(s => s.Length == 2)
            .ToDictionary(s => s[0].ToUpperInvariant(), s => s[1]);

        if (details.ContainsKey("DISTRIB_ID"))
        {
            SystemDistroId = SystemDistro = details["DISTRIB_ID"];
        }
        else if(details.ContainsKey("ID"))
        {
            SystemDistroId = details["ID"];
        }

        if (details.ContainsKey("NAME"))
        {
            SystemDistro = details["NAME"];
        }

        if (details.ContainsKey("DISTRIB_RELEASE"))
        {
            SystemVersion = details["DISTRIB_RELEASE"];
        }
        else if (details.ContainsKey("BUILD_ID"))
        {
            SystemVersion = details["BUILD_ID"];
        }
    }
}