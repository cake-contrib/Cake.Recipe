public static bool TransifexUserSettingsExists()
{
    var path = GetTransifexUserSettingsPath();
    return FileExists(path);
}

public static string GetTransifexUserSettingsPath()
{
    var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/.transifexrc");
    return path;
}

public static bool TransifexIsConfiguredForRepository()
{
    return FileExists("./.tx/config");
}

// Before we do anything with transifex, we must make sure that it have been properly
// Initialized, this is mostly related to running on appveyor or other CI.
// Because we expect the repository to already be configured to use
// transifex, we cannot run tx init, or it would replace the repository configuration file.
BuildTasks.TransifexSetupTask = Task("Transifex-Setup")
    .WithCriteria(() => BuildParameters.TransifexEnabled)
    .WithCriteria(() => !TransifexUserSettingsExists())
    .WithCriteria(() => BuildParameters.Transifex.HasCredentials)
    .Does(() =>
    {
        var path = GetTransifexUserSettingsPath();
        var encoding = new System.Text.UTF8Encoding(false);
        const string text = "[https://www.transifex.com]\r\nhostname = https://www.transifex.com\r\npassword = " + BuildParameters.Transifex.ApiToken + "\r\nusername = api";
        System.IO.File.WriteAllText(path, text, encoding);
    });

BuildTasks.TransifexPushSourceResource = Task("Transifex-Push-SourceFiles")
    .WithCriteria(() => BuildParameters.TransifexEnabled)
    .WithCriteria(() => BuildParameters.IsRunningOnAppveyor || string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase))
    .IsDependentOn("Transifex-Setup")
    .Does(() =>
    {
        // TODO: Allow the usage of force, perhaps when target have been explicitly called.
        TransifexPush(new TransifexPushSettings {
            UploadSourceFiles = true
        });
    });