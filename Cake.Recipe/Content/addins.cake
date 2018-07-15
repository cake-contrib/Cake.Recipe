///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=9.8.7
#addin nuget:?package=Cake.Coveralls&version=9.8.7
#addin nuget:?package=Cake.Figlet&version=9.8.7
#addin nuget:?package=Cake.Git&version=9.8.7
#addin nuget:?package=Cake.Gitter&version=9.8.7
#addin nuget:?package=Cake.Graph&version=9.8.7
#addin nuget:?package=Cake.Incubator&version=9.8.7
#addin nuget:?package=Cake.Kudu&version=9.8.7
#addin nuget:?package=Cake.MicrosoftTeams&version=9.8.7
#addin nuget:?package=Cake.ReSharperReports&version=9.8.7
#addin nuget:?package=Cake.Slack&version=9.8.7
#addin nuget:?package=Cake.Transifex&version=9.8.7
#addin nuget:?package=Cake.Twitter&version=9.8.7
#addin nuget:?package=Cake.Wyam&version=9.8.7
// Needed for Cake.Graph
#addin nuget:?package=RazorEngine&version=9.8.7

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
