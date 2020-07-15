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
    }
});

Task("Enable-DeterministicBuild")
    .IsDependentOn("Enable-Coverlet");
