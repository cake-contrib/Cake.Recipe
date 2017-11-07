#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Git&Version=0.16.1"

////////////////////////////////////////////////////////
/// Global variables
////////////////////////////////////////////////////////
var recipePath          = MakeAbsolute(Directory("./BuildArtifacts/Packages/NuGet"));
var testReposRootPath   = MakeAbsolute(Directory("./tests/integration/repos"));
var recipeVersion       = Argument("recipe-version", "*");
var exceptions          = new List<Exception>();
var testRepos           = new [] {
                            new {
                                Path = testReposRootPath.Combine("Cake.Gulp"),
                                Url = "https://github.com/cake-contrib/Cake.Gulp.git"
                            },
                            new {
                                Path = testReposRootPath.Combine("Cake.Http"),
                                Url = "https://github.com/cake-contrib/Cake.Http.git"
                            },
                            new {
                                Path = testReposRootPath.Combine("Cake.Unity"),
                                Url = "https://github.com/cake-contrib/Cake.Unity.git"
                            }
                          };
var package             = GetFiles(
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
    .Does(() => {
        if (DirectoryExists(testReposRootPath))
        {
            ForceDeleteDirectory(testReposRootPath.FullPath);
        }
        EnsureDirectoryExists(testReposRootPath);
});

var cloneTask = Task("Clone-Repositories")
    .IsDependentOn("Clean")
    .Does(() => {
        Information("Clone complete.");
});

var unzipTask = Task("Unzip-Cake-Recipe")
    .IsDependentOn("Clone-Repositories")
    .Does(() => {
        Information("Unzip Cake recipe done.");
});

var testsTask = Task("Tests")
    .IsDependentOn("Unzip-Cake-Recipe")
    .Does(() => {
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
foreach(var testRepo in testRepos)
{
    var url = testRepo.Url;
    var path = testRepo.Path;
    var pkg = package;
    var pkgName = pkg.GetFilename();
    var name = path.GetDirectoryName();
    var exs = exceptions;

    cloneTask.IsDependentOn(
        Task("Clone: " + name)
            .Does(context => {
                context.Information("Cloning {0}...", url);
                context.GitClone(url, path);
    }));

    unzipTask.IsDependentOn(
        Task("Unzip: " + name)
            .Does(context => {
                var testReposRecipePath = path.Combine("tools").Combine("Cake.Recipe");
                context.Information("Unzipping nuget {0} to {1}...", pkgName, testReposRecipePath);
                context.EnsureDirectoryExists(testReposRecipePath);
                context.Unzip(pkg, testReposRecipePath);
    }));

    testsTask.IsDependentOn(
        Task("Tests: " + name)
            .Does(context => {
                try
                {
                    var setupCakePath = path.CombineWithFilePath("setup.cake");
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

Setup(context => {
    Information("Starting integration tests...");
});

Teardown(context => {

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