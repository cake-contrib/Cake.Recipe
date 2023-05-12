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

BuildParameters.Tasks.TransifexPushSourceResource = Task("Transifex-Push-SourceFiles")
    .WithCriteria(() => BuildParameters.CanPushTranslations)
    .WithCriteria((ctx) => BuildParameters.Transifex.HasCredentials || TransifexUserSettingsExists(ctx), "No transifex credentials specified")
    .Does(() =>
    {
        var settings = new TransifexPushSettings
        {
            UploadSourceFiles = true,
            Force             = string.Equals(BuildParameters.Target, "Transifex-Push-SourceFiles", StringComparison.OrdinalIgnoreCase)
        };

        if (!TransifexUserSettingsExists(Context))
        {
            settings.ArgumentCustomization = (args) => args.PrependSwitchQuoted("--token", BuildParameters.Transifex.ApiToken);
        }

        TransifexPush(settings);
    });

BuildParameters.Tasks.TransifexPullTranslations = Task("Transifex-Pull-Translations")
    .WithCriteria(() => BuildParameters.CanPullTranslations)
    .WithCriteria((ctx) => BuildParameters.Transifex.HasCredentials || TransifexUserSettingsExists(ctx), "No transifex credentials specified")
    .IsDependentOn("Transifex-Push-SourceFiles")
    .Does(() =>
    {
        var settings = new TransifexPullSettings
        {
            All               = true,
            Mode              = BuildParameters.TransifexPullMode,
            MinimumPercentage = BuildParameters.TransifexPullPercentage
        };

        if (BuildParameters.Transifex.HasCredentials)
        {
            settings.ArgumentCustomization = (args) => args.PrependSwitchQuoted("--token", BuildParameters.Transifex.ApiToken);
        }

        TransifexPull(settings);
    });

BuildParameters.Tasks.TransifexPushTranslations = Task("Transifex-Push-Translations")
    .Does(() =>
    {
        var settings = new TransifexPushSettings
        {
            UploadTranslations = true
        };

        if (BuildParameters.Transifex.HasCredentials)
        {
            settings.ArgumentCustomization = (args) => args.PrependSwitchQuoted("--token", BuildParameters.Transifex.ApiToken);
        }

        TransifexPush(settings);
    });
