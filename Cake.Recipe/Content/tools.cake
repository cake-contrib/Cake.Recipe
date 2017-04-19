///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

private const string CoverallsTool = "#tool nuget:?package=coveralls.io&version=1.3.4";
private const string GitReleaseManagerTool = "#tool nuget:?package=gitreleasemanager&version=0.5.0";
private const string GitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=3.6.2";
private const string ReSharperTools = "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2016.3.20161223.160402";
private const string ReSharperReportsTool = "#tool nuget:?package=ReSharperReports&version=0.2.0";
private const string KuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.3.1";
private const string WyamTool = "#tool nuget:?package=Wyam&version=0.17.7";
private const string GitLinkTool = "#tool nuget:?package=gitlink&version=2.4.1";
private const string MSBuildExtensionPackTool = "#tool nuget:?package=MSBuild.Extension.Pack&version=1.9.0";
private const string XUnitTool = "#tool nuget:?package=xunit.runner.console&version=2.1.0";
private const string NUnitTool = "#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.1";
private const string OpenCoverTool = "#tool nuget:?package=OpenCover&version=4.6.519";
private const string ReportGeneratorTool = "#tool nuget:?package=ReportGenerator&version=2.4.5";
private const string ReportUnitTool = "#tool nuget:?package=ReportUnit&version=1.2.1";
private const string FixieTool = "#tool nuget:?package=Fixie&version=1.0.2";

Action<string, Action> RequireTool = (tool, action) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, tool);
        CakeExecuteScript(script);
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
