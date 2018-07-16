///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=0.4.0
#addin nuget:?package=Cake.Coveralls&version=0.9.0
#addin nuget:?package=Cake.Figlet&version=1.1.0
#addin nuget:?package=Cake.Git&version=0.18.0
#addin nuget:?package=Cake.Gitter&version=0.9.0
#addin nuget:?package=Cake.Graph&version=0.6.0
#addin nuget:?package=Cake.Incubator&version=2.0.2
#addin nuget:?package=Cake.Kudu&version=0.8.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.7.0
#addin nuget:?package=Cake.ReSharperReports&version=0.10.0
#addin nuget:?package=Cake.Slack&version=0.12.0
#addin nuget:?package=Cake.Transifex&version=0.7.0
#addin nuget:?package=Cake.Twitter&version=0.8.0
#addin nuget:?package=Cake.Wyam&version=1.4.1
#addin nuget:?package=Cake.Issues&version=0.3.1
#addin nuget:?package=Cake.Issues.MsBuild&version=0.3.1
#addin nuget:?package=Cake.Issues.InspectCode&version=0.3.0
#addin nuget:?package=Cake.Issues.Reporting&version=0.3.0
#addin nuget:?package=Cake.Issues.Reporting.Generic&version=0.3.4
// Needed for Cake.Graph
#addin nuget:?package=RazorEngine&version=3.10.0&loaddependencies=true

Action<string, IDictionary<string, string>> RequireAddin = (code, envVars) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, code);
        var arguments = new Dictionary<string, string>();

        if(BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if(BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        CakeExecuteScript(script,
            new CakeSettings
            {
                EnvironmentVariables = envVars,
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
};
