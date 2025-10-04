#addin nuget:?package=Cake.FileHelpers&version=6.1.3
#addin nuget:?package=Cake.Git&version=3.0.0

////////////////////////////////////////////////////////
/// Global variables
////////////////////////////////////////////////////////
var recipePath                    = MakeAbsolute(Directory("./BuildArtifacts/Packages/NuGet"));
var recipeVersion                 = Argument("recipe-version", "*");
var shouldCreateGlobalReposFolder = Argument("shouldCreateGlobalReposFolder", true);
var testReposRootPath             = MakeAbsolute(Directory(shouldCreateGlobalReposFolder ? "c:/CakeRecipeTests/repos" : "./tests/repos"));
var exceptions                    = new List<Exception>();
var testRepos                     = new [] {
                                      new {
                                          Path = testReposRootPath.Combine("Cake.Gulp"),
                                          Url = "https://github.com/cake-contrib/Cake.Gulp.git",
                                          BuildScriptName = "recipe.cake"
                                      },
                                      new {
                                          Path = testReposRootPath.Combine("Cake.Http"),
                                          Url = "https://github.com/cake-contrib/Cake.Http.git",
                                          BuildScriptName = "recipe.cake"
                                      },
                                      new {
                                          Path = testReposRootPath.Combine("Cake.Twitter"),
                                          Url = "https://github.com/cake-contrib/Cake.Twitter.git",
                                          BuildScriptName = "recipe.cake"
                                      }
                                  };

var package                       = GetFiles(
                                        string.Concat(
                                            recipePath,
                                            "/Cake.Recipe." + recipeVersion + ".nupkg"
                                        )
                                    ).FirstOrDefault();
if (package == null)
{
    throw new Exception("Failed to find Cake Recipe NuGet Package");
}

////////////////////////////////////////////////////////
/// Main tasks
////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    if (DirectoryExists(testReposRootPath))
    {
        ForceDeleteDirectory(testReposRootPath.FullPath);
    }

    EnsureDirectoryExists(testReposRootPath);
});

var cloneTask = Task("Clone-Repositories")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Clone complete.");
});

var updateFileTask = Task("Update-Repo-Cake-Recipe-File")
    .IsDependentOn("Clone-Repositories")
    .Does(() =>
{
    Information("Update Repo Cake.Recipe File done.");
});

var testsTask = Task("Tests")
    .IsDependentOn("Update-Repo-Cake-Recipe-File")
    .Does(() =>
{
    if (exceptions.Any())
    {
        throw new AggregateException("Integration tests failed", exceptions);
    }

    Information("Tests complete.");
});

////////////////////////////////////////////////////////
/// Dynamic tasks
////////////////////////////////////////////////////////

Information("Setting up integration tests...");

foreach (var testRepo in testRepos)
{
    var url = testRepo.Url;
    var path = testRepo.Path;
    var name = path.GetDirectoryName();
    var exs = exceptions;

    cloneTask.IsDependentOn(
        Task("Clone: " + name)
            .Does(context =>
    {
        context.Information("Cloning {0}...", url);
        context.GitClone(url, path);
    }));

    updateFileTask.IsDependentOn(
        Task("Update file for: " + name)
            .Does(context =>
    {
        // Here we want to search the entry cake file for the load preprocessor directive that is loading Cake.Recipe
        // i.e. change this #load nuget:?package=Cake.Recipe&prerelease
        // #load nuget:file://<path_to_where_newly_generated_nupkg_lives>?package=Cake.Recipe&prerelease

        ReplaceRegexInFiles(
            path.CombineWithFilePath(testRepo.BuildScriptName).FullPath,
            @"\s*#l(oad)?.*Cake\.Recipe.*",
            string.Format("#load nuget:file://{0}?package=Cake.Recipe&prerelease", recipePath));
    }));

    testsTask.IsDependentOn(
        Task("Tests: " + name)
            .Does(context =>
    {
        try
        {
            var setupCakePath = path.CombineWithFilePath(testRepo.BuildScriptName);
            context.Information("Testing {0}...", setupCakePath);
            context.CakeExecuteScript(setupCakePath,
                    new CakeSettings {
                        Arguments = new Dictionary<string, string>{
                            { "verbosity", context.Log.Verbosity.ToString("F") }
                }});
        }
        catch(Exception ex)
        {
            Error("{0}: {1}", name, ex);
            exs.Add(new Exception(
                    testRepo.Url,
                    ex
                ));
        }
    }));
}

Setup(context =>
{
    Information("Starting integration tests...");
});

Teardown(context =>
{
});

RunTarget("Tests");

public static void ForceDeleteDirectory(string path)
{
    var directory = new System.IO.DirectoryInfo(path) { Attributes = FileAttributes.Normal };

    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
    {
        info.Attributes = FileAttributes.Normal;
    }

    directory.Delete(true);
}
