#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "Cake.Recipe",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Recipe",
                            appVeyorAccountName: "cakecontrib");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

Task("Run-Integration-Tests")
    .IsDependentOn("Default")
    .Does(() => {
    CakeExecuteScript("./test.cake",
        new CakeSettings {
            Arguments = new Dictionary<string, string>{
                { "recipe-version", BuildParameters.Version.SemVersion },
                { "verbosity", Context.Log.Verbosity.ToString("F") }
            }});
});

Build.RunNuGet();