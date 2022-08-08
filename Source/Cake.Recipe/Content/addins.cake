///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Codecov&version=1.0.1
#addin nuget:?package=Cake.Coveralls&version=1.1.0
#addin nuget:?package=Cake.Coverlet&version=2.5.4
#addin nuget:?package=Portable.BouncyCastle&version=1.8.5
#addin nuget:?package=MimeKit&version=2.9.1
#addin nuget:?package=MailKit&version=2.8.0
#addin nuget:?package=MimeTypesMap&version=1.0.8
#addin nuget:?package=Cake.Email.Common&version=0.4.2
#addin nuget:?package=Cake.Email&version=1.0.2
#addin nuget:?package=Cake.Figlet&version=2.0.1
#addin nuget:?package=Cake.Gitter&version=1.1.0
#addin nuget:?package=Cake.Incubator&version=6.0.0
#addin nuget:?package=Cake.Kudu&version=1.0.1
#addin nuget:?package=Cake.MicrosoftTeams&version=1.0.1
#addin nuget:?package=Cake.Slack&version=1.0.1
#addin nuget:?package=Cake.Transifex&version=1.0.1
#addin nuget:?package=Cake.Twitter&version=1.0.0
#addin nuget:?package=Cake.Wyam&version=2.2.12

#load nuget:?package=Cake.Issues.Recipe&version=1.3.2

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
