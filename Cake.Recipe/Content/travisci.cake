BuildParameters.Tasks.PrintTravisCIEnvironmentVariablesTask = Task("Print-TravisCI-Environment-Variables")
    .WithCriteria(() => TravisCI.IsRunningOnTravisCI)
    .Does(() =>
    {
        using(var travisCI = TravisCI.Fold("Environment Variables"))
        {
            Information("");
        }
    });

BuildParameters.Tasks.UploadTravisCIArtifactsTask = Task("Upload-TravisCI-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildParameters.IsRunningOnTravisCI)
    .WithCriteria(() => DirectoryExists(BuildParameters.Directories.NuGetPackages))
    .Does(() =>
    {    
        using(var travisCI = TravisCI.Fold("Upload"))
        // Concatenating FilePathCollections should make sure we get unique FilePaths
        foreach(var package in GetFiles(BuildParameters.Paths.Directories.Packages + "/**/*") +
                            GetFiles(BuildParameters.Paths.Directories.NuGetPackages + "/*"))
        {
            TravisCI.UploadArtifact(package);
        }
    });

BuildParameters.Task.ClearTravisCICacheTask = Task("Clear-TravisCI-Cache")
    .Does(() => 
    {

    });