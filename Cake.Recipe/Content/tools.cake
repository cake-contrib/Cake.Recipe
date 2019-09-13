///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

private const string CodecovTool = "#tool nuget:?package=codecov&version=1.4.0";
private const string CoverallsTool = "#tool nuget:?package=coveralls.io&version=1.4.2";
private const string GitReleaseManagerTool = "#tool nuget:?package=GitReleaseManager&version=0.8.0";
private const string GitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=5.0.1";
private const string ReSharperTools = "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2019.2.2";
private const string ReSharperReportsTool = "#tool nuget:?package=ReSharperReports&version=0.4.0";
private const string KuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.5.2";
private const string WyamTool = "#tool nuget:?package=Wyam&version=2.2.5";
private const string GitLinkTool = "#tool nuget:?package=gitlink&version=3.1.0";
private const string MSBuildExtensionPackTool = "#tool nuget:?package=MSBuild.Extension.Pack&version=1.9.1";
private const string XUnitTool = "#tool nuget:?package=xunit.runner.console&version=2.4.1";
private const string NUnitTool = "#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0";
private const string OpenCoverTool = "#tool nuget:?package=OpenCover&version=4.7.922";
private const string ReportGeneratorTool = "#tool nuget:?package=ReportGenerator&version=4.2.19";
private const string ReportUnitTool = "#tool nuget:?package=ReportUnit&version=1.2.1";
private const string FixieTool = "#tool nuget:?package=Fixie&version=2.1.1";

Action<string, Action> RequireTool = (tool, action) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        var arguments = new Dictionary<string, string>();

        if(BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if(BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        System.IO.File.WriteAllText(script.FullPath, tool);
        CakeExecuteScript(script,
            new CakeSettings
            {
                Arguments = arguments
            });
    }
    finally
    {
        if (FileExists(script))
        {
            DeleteFile(script);
        }
    }

    action();
};
