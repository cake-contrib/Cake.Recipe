///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.CodeAnalysisReporting&version=0.1.1
#addin nuget:?package=Cake.Codecov&version=0.2.0
#addin nuget:?package=Cake.Coveralls&version=0.4.0
#addin nuget:?package=Cake.Figlet&version=0.4.0
#addin nuget:?package=Cake.Git&version=0.15.0
#addin nuget:?package=Cake.Gitter&version=0.5.0
#addin nuget:?package=Cake.Graph&version=0.2.0
#addin nuget:?package=Cake.Incubator&version=1.3.0
#addin nuget:?package=Cake.Kudu&version=0.4.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.3.0
#addin nuget:?package=Cake.ReSharperReports&version=0.6.0
#addin nuget:?package=Cake.Slack&version=0.6.0
#addin nuget:?package=Cake.Twitter&version=0.4.0
#addin nuget:?package=Cake.Wyam&version=0.17.7

Action<string, IDictionary<string, string>> RequireAddin = (code, envVars) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, code);
        CakeExecuteScript(script, new CakeSettings{ EnvironmentVariables = envVars });
    }
    finally
    {
        if (FileExists(script))
        {
            DeleteFile(script);
        }
    }
};
