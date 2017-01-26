///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void ExecuteGitLink()
{
    Information("Starting GitLink Execution...");

    GitLink(BuildParameters.RootDirectoryPath, new GitLinkSettings {
        SolutionFileName = string.Concat(BuildParameters.SourceDirectoryPath.GetDirectoryName(), "/", BuildParameters.SolutionFilePath.GetFilename())
    });

    Information("GitLink Execution completed.");
}