///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=3.0.0
#addin nuget:?package=Cake.Coveralls&version=3.0.0
#addin nuget:?package=Cake.Coverlet&version=3.0.4
#addin nuget:?package=Portable.BouncyCastle&version=1.8.5
#addin nuget:?package=Cake.Email&version=2.0.0&loaddependencies=true
#addin nuget:?package=Cake.Incubator&version=8.0.0
#addin nuget:?package=Cake.MicrosoftTeams&version=2.0.0
#addin nuget:?package=Cake.Slack&version=2.0.0
#addin nuget:?package=Cake.Transifex&version=2.0.0
#addin nuget:?package=Cake.Twitter&version=3.0.0
#addin nuget:?package=Cake.Mastodon&version=1.1.0

#load nuget:?package=Cake.Issues.Recipe&version=3.1.1

Action<string, IDictionary<string, string>> RequireAddin = (code, envVars) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        System.IO.File.WriteAllText(script.FullPath, code);
        var arguments = new Dictionary<string, string>();

        if (BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if (BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
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
