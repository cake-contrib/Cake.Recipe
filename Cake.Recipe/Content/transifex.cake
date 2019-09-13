public static bool TransifexUserSettingsExists(ICakeContext context)
{
    var path = GetTransifexUserSettingsPath();
    return context.FileExists(path);
}

public static string GetTransifexUserSettingsPath()
{
    var path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/.transifexrc");
    return path;
}

public static bool TransifexIsConfiguredForRepository(ICakeContext context)
{
    return context.FileExists("./.tx/config");
}

// Before we do anything with transifex, we must make sure that it has been properly
// Initialized, this is mostly related to running on appveyor or other CI.
// Because we expect the repository to already be configured to use
// transifex, we cannot run tx init, or it would replace the repository configuration file.
BuildParameters.Tasks.TransifexSetupTask = Task("Transifex-Setup")
    .WithCriteria(() => BuildParameters.TransifexEnabled, "Transifex is not enabled")
    .WithCriteria(() => !TransifexUserSettingsExists(Context), "Transifex user settings already exist")
    .WithCriteria(() => BuildParameters.Transifex.HasCredentials, "Missing transifex credentials")
    .Does(() =>
    {
        var path = GetTransifexUserSettingsPath();
        var encoding = new System.Text.UTF8Encoding(false);
        var text = string.Format("[https://www.transifex.com]\r\nhostname = https://www.transifex.com\r\npassword = {0}\r\nusername = api", BuildParameters.Transifex.ApiToken);
        System.IO.File.WriteAllText(path, text, encoding);
    });

BuildParameters.Tasks.TransifexPushSourceResource = Task("Transifex-Push-SourceFiles")
    .WithCriteria(() => BuildParameters.CanPushTranslations)
    .IsDependentOn("Transifex-Setup")
    .Does(() =>
    {
        TransifexPush(new TransifexPushSettings {
            UploadSourceFiles = true,
            Force = string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase)
        });
    });

BuildParameters.Tasks.TransifexPullTranslations = Task("Transifex-Pull-Translations")
    .WithCriteria(() => BuildParameters.CanPullTranslations)
    .IsDependentOn("Transifex-Push-SourceFiles")
    .Does(() =>
    {
        TransifexPull(new TransifexPullSettings {
            All = true,
            Mode = BuildParameters.TransifexPullMode,
            MinimumPercentage = BuildParameters.TransifexPullPercentage
        });
    });

BuildParameters.Tasks.TransifexPushTranslations = Task("Transifex-Push-Translations")
    .Does(() =>
    {
        TransifexPush(new TransifexPushSettings {
            UploadTranslations = true
        });
    });
