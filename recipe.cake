// todo: temporarily switch to local sources
//#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

#load "Cake.Recipe/Content/addins.cake"
#load "Cake.Recipe/Content/analyzing.cake"
#load "Cake.Recipe/Content/appveyor.cake"
#load "Cake.Recipe/Content/azurepipelines.cake"
#load "Cake.Recipe/Content/build.cake"
#load "Cake.Recipe/Content/buildData.cake"
#load "Cake.Recipe/Content/buildProvider.cake"
#load "Cake.Recipe/Content/chocolatey.cake"
#load "Cake.Recipe/Content/codecov.cake"
#load "Cake.Recipe/Content/configuration.cake"
#load "Cake.Recipe/Content/coveralls.cake"
#load "Cake.Recipe/Content/credentials.cake"
#load "Cake.Recipe/Content/environment.cake"
#load "Cake.Recipe/Content/gitlink.cake"
#load "Cake.Recipe/Content/gitreleasemanager.cake"
#load "Cake.Recipe/Content/gitter.cake"
#load "Cake.Recipe/Content/gitversion.cake"
#load "Cake.Recipe/Content/microsoftteams.cake"
#load "Cake.Recipe/Content/nuget.cake"
#load "Cake.Recipe/Content/packages.cake"
#load "Cake.Recipe/Content/parameters.cake"
#load "Cake.Recipe/Content/paths.cake"
#load "Cake.Recipe/Content/slack.cake"
#load "Cake.Recipe/Content/tasks.cake"
#load "Cake.Recipe/Content/testing.cake"
#load "Cake.Recipe/Content/tools.cake"
#load "Cake.Recipe/Content/toolsettings.cake"
#load "Cake.Recipe/Content/transifex.cake"
#load "Cake.Recipe/Content/twitter.cake"
#load "Cake.Recipe/Content/version.cake"
#load "Cake.Recipe/Content/wyam.cake"

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "Cake.Recipe",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Recipe",
                            appVeyorAccountName: "cakecontrib",
                            shouldRunGitVersion: true);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.Tasks.CleanTask
    .IsDependentOn("Generate-Version-File");

Task("Generate-Version-File")
    .Does(() => {
        var buildMetaDataCodeGen = TransformText(@"
        public class BuildMetaData
        {
            public static string Date { get; } = ""<%date%>"";
            public static string Version { get; } = ""<%version%>"";
        }",
        "<%",
        "%>"
        )
   .WithToken("date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
   .WithToken("version", BuildParameters.Version.SemVersion)
   .ToString();

    System.IO.File.WriteAllText(
        "./Cake.Recipe/Content/version.cake",
        buildMetaDataCodeGen
        );
    });

Task("Run-Local-Integration-Tests")
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
