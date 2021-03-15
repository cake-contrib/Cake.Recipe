///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=0.9.1
#addin nuget:?package=Cake.Coveralls&version=0.10.2
#addin nuget:?package=Cake.Coverlet&version=2.5.1
#addin nuget:?package=Portable.BouncyCastle&version=1.8.5
#addin nuget:?package=MimeKit&version=2.9.1
#addin nuget:?package=MailKit&version=2.8.0
#addin nuget:?package=MimeTypesMap&version=1.0.8
#addin nuget:?package=Cake.Email.Common&version=0.4.2
#addin nuget:?package=Cake.Email&version=0.10.0
#addin nuget:?package=Cake.Figlet&version=1.3.1
#addin nuget:?package=Cake.Gitter&version=0.11.1
#addin nuget:?package=Cake.Incubator&version=5.1.0
#addin nuget:?package=Cake.Kudu&version=0.11.0
#addin nuget:?package=Cake.MicrosoftTeams&version=0.9.0
#addin nuget:?package=Cake.Slack&version=0.13.0
#addin nuget:?package=Cake.Transifex&version=0.9.1
#addin nuget:?package=Cake.Twitter&version=0.10.1
#addin nuget:?package=Cake.Wyam&version=2.2.9

#load nuget:?package=Cake.Issues.Recipe&version=0.4.3

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
