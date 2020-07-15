public void AddNetCorePackage(ICakeContext context, string projectName, string projectPath, string packageName)
{
    var dotnetTool = context.Tools.Resolve("dotnet.exe");
    if (dotnetTool == null) {
        dotnetTool = context.Tools.Resolve("dotnet");
    }
    context.Information("Adding package '{0}' to project '{1}'!", packageName, projectName);

    StartProcess(dotnetTool, new ProcessSettings()
        .WithArguments(builder =>
            builder.Append("add")
                   .Append(projectPath)
                   .Append("package")
                   .Append(packageName)));
}

Task("Enable-Coverlet")
    .Does(() =>
{
    var force = HasArgument("force");
    var rootPath = BuildParameters.RootDirectoryPath;
    var configFile = rootPath.CombineWithFilePath(".git/config");
    if (!FileExists(configFile))
    {
        rootPath = GitFindRootFromPath(rootPath);
        configFile = rootPath.CombineWithFilePath(".git/config");
    }

    Information("Git Config: " + configFile);

    // This is a very naive check of which git provider is being used.
    // But should be good enough for most cases.
    var content = System.IO.File.ReadAllText(configFile.ToString());
    var packages = new List<string>();
    if (content.Contains("github.com"))
    {
        packages.Add("Microsoft.SourceLink.GitHub");
    }

    if (content.Contains("gitlab.com"))
    {
        packages.Add("Microsoft.SourceLink.GitLab");
    }

    if (content.Contains("bitbucket.org"))
    {
        packages.Add("Microsoft.SourceLink.BitBucket.Git");
    }

    if (content.Contains("azure.com") || content.Contains("visualstudio.com"))
    {
        packages.Add("Microsoft.SourceLink.AzureRepos.Git");
    }

    if (!packages.Any())
    {
        Warning("We was unable to determine which package to add for supporting SourceLink");
        Warning("Please see <https://github.com/dotnet/sourcelink#using-source-link-in-net-projects> for adding this manually.");
        Warning("We are adding the common package instead, remove this if it is not appropriate.");
        packages.Add("Microsoft.SourceLink.Common");
    }

    var projects = ParseSolution(BuildParameters.SolutionFilePath).GetProjects();

    foreach (var project in projects)
    {
        var parsedProject = ParseProject(project.Path, BuildParameters.Configuration);

        if (!parsedProject.IsNetCore && !parsedProject.IsVS2017ProjectFormat )
            continue;

        if (parsedProject.IsDotNetCliTestProject())
        {
            var coverletPackage = parsedProject.GetPackage("coverlet.msbuild");
            if (force || coverletPackage == null || !Version.TryParse(coverletPackage.Version, out Version pkgVersion) || pkgVersion < Version.Parse("2.9.0"))
            {
                AddNetCorePackage(Context, parsedProject.RootNameSpace, project.Path.ToString(), "coverlet.msbuild");
            }
        }

        foreach (var package in packages)
        {
            if (force || !parsedProject.HasPackage(package))
            {
                AddNetCorePackage(Context, parsedProject.RootNameSpace, project.Path.ToString(), package);
            }
        }
    }
});

Task("Enable-DeterministicBuild")
    .IsDependentOn("Enable-Coverlet");
