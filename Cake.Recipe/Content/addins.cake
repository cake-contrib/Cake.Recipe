///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=0.7.0
#addin nuget:?package=Cake.Coveralls&version=0.10.1
#addin nuget:?package=Cake.Email&version=0.8.0&loaddependencies=true // loading dependencies is important to ensure Cake.Email.Common is loaded as well
#addin nuget:?package=Cake.Figlet&version=1.3.1
#addin nuget:?package=Cake.Git&version=0.21.0
#addin nuget:?package=Cake.Gitter&version=0.11.1
#addin nuget:?package=Cake.Graph&version=0.8.0
#addin nuget:?package=Cake.Incubator&version=5.1.0
#addin nuget:?package=Cake.Kudu&version=0.10.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.9.0
#addin nuget:?package=Cake.ReSharperReports&version=0.11.0
#addin nuget:?package=Cake.Slack&version=0.13.0
#addin nuget:?package=Cake.Transifex&version=0.8.0
#addin nuget:?package=Cake.Twitter&version=0.10.0
#addin nuget:?package=Cake.Wyam&version=2.2.5
#addin nuget:?package=Cake.Issues&version=0.7.1
#addin nuget:?package=Cake.Issues.MsBuild&version=0.7.0
#addin nuget:?package=Cake.Issues.InspectCode&version=0.7.1
#addin nuget:?package=Cake.Issues.Reporting&version=0.7.0
#addin nuget:?package=Cake.Issues.Reporting.Generic&version=0.7.1

// TODO: Conditionally decide whether to install packages or not
#addin nuget:?package=Cake.Issues.PullRequests&version=0.7.0
#addin nuget:?package=Cake.Issues.PullRequests.AppVeyor&version=0.7.0

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
