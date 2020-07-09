#load "./includes.cake"

public class BuildMetaData
{
    public static string Date { get; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    public static string Version { get; } = "DogFood";
    public static string CakeVersion { get; } = typeof(ICakeContext).Assembly.GetName().Version.ToString();
}

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
    .Does<BuildVersion>((context, buildVersion) => {
        var buildMetaDataCodeGen = TransformText(@"
        public class BuildMetaData
        {
            public static string Date { get; } = ""<%date%>"";
            public static string Version { get; } = ""<%version%>"";
            public static string CakeVersion { get; } = ""<%cakeversion%>"";
        }",
        "<%",
        "%>"
        )
   .WithToken("date", BuildMetaData.Date)
   .WithToken("version", buildVersion.SemVersion)
   .WithToken("cakeversion", BuildMetaData.CakeVersion)
   .ToString();

    System.IO.File.WriteAllText(
        "./Cake.Recipe/Content/version.cake",
        buildMetaDataCodeGen
        );
    });

Task("Run-Local-Integration-Tests")
    .IsDependentOn("Default")
    .Does<BuildVersion>((context, buildVersion) => {
    CakeExecuteScript("./test.cake",
        new CakeSettings {
            Arguments = new Dictionary<string, string>{
                { "recipe-version", buildVersion.SemVersion },
                { "verbosity", Context.Log.Verbosity.ToString("F") }
            }});
});

Build.RunNuGet();
