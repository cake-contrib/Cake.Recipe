public class BuildTasks
{
    public CakeTaskBuilder<ActionTask> DupFinderTask { get; set; }
    public CakeTaskBuilder<ActionTask> InspectCodeTask { get; set; }
    public CakeTaskBuilder<ActionTask> AnalyzeTask { get; set; }
    public CakeTaskBuilder<ActionTask> PrintAppVeyorEnvironmentVariablesTask { get; set; }
    public CakeTaskBuilder<ActionTask> UploadAppVeyorArtifactsTask { get; set; }
    public CakeTaskBuilder<ActionTask> ClearAppVeyorCacheTask { get; set; }
    public CakeTaskBuilder<ActionTask> ShowInfoTask { get; set; }
    public CakeTaskBuilder<ActionTask> CleanTask { get; set; }
    public CakeTaskBuilder<ActionTask> DotNetCoreCleanTask { get; set; }
    public CakeTaskBuilder<ActionTask> RestoreTask { get; set; }
    public CakeTaskBuilder<ActionTask> DotNetCoreRestoreTask { get; set; }
    public CakeTaskBuilder<ActionTask> BuildTask { get; set; }
    public CakeTaskBuilder<ActionTask> DotNetCoreBuildTask { get; set; }
    public CakeTaskBuilder<ActionTask> PackageTask { get; set; }
    public CakeTaskBuilder<ActionTask> DefaultTask { get; set; }
    public CakeTaskBuilder<ActionTask> AppVeyorTask { get; set; }
    public CakeTaskBuilder<ActionTask> ReleaseNotesTask { get; set; }
    public CakeTaskBuilder<ActionTask> ClearCacheTask { get; set; }
    public CakeTaskBuilder<ActionTask> PreviewTask { get; set; }
    public CakeTaskBuilder<ActionTask> PublishDocsTask { get; set; }
    public CakeTaskBuilder<ActionTask> CreateChocolateyPackagesTask { get; set; }
    public CakeTaskBuilder<ActionTask> PublishChocolateyPackagesTask { get; set; }
    public CakeTaskBuilder<ActionTask> UploadCodecovReportTask { get; set; }
    public CakeTaskBuilder<ActionTask> UploadCoverallsReportTask { get; set; }
    public CakeTaskBuilder<ActionTask> UploadCoverageReportTask { get; set; }
    public CakeTaskBuilder<ActionTask> CreateReleaseNotesTask { get; set; }
    public CakeTaskBuilder<ActionTask> ExportReleaseNotesTask { get; set; }
    public CakeTaskBuilder<ActionTask> PublishGitHubReleaseTask { get; set; }
    public CakeTaskBuilder<ActionTask> DotNetCorePackTask { get; set; }
    public CakeTaskBuilder<ActionTask> CreateNuGetPackageTask { get; set; }
    public CakeTaskBuilder<ActionTask> CreateNuGetPackagesTask { get; set; }
    public CakeTaskBuilder<ActionTask> PublishMyGetPackagesTask { get; set; }
    public CakeTaskBuilder<ActionTask> PublishNuGetPackagesTask { get; set; }
    public CakeTaskBuilder<ActionTask> InstallReportGeneratorTask { get; set; }
    public CakeTaskBuilder<ActionTask> InstallReportUnitTask { get; set; }
    public CakeTaskBuilder<ActionTask> InstallOpenCoverTask { get; set; }
    public CakeTaskBuilder<ActionTask> TestNUnitTask { get; set; }
    public CakeTaskBuilder<ActionTask> TestxUnitTask { get; set; }
    public CakeTaskBuilder<ActionTask> TestMSTestTask { get; set; }
    public CakeTaskBuilder<ActionTask> TestVSTestTask { get; set; }
    public CakeTaskBuilder<ActionTask> TestFixieTask { get; set; }
    public CakeTaskBuilder<ActionTask> TransifexPullTranslations { get; set; }
    public CakeTaskBuilder<ActionTask> TransifexPushSourceResource { get; set; }
    public CakeTaskBuilder<ActionTask> TransifexPushTranslations { get; set; }
    public CakeTaskBuilder<ActionTask> TransifexSetupTask { get; set; }
    public CakeTaskBuilder<ActionTask> DotNetCoreTestTask { get; set; }
    public CakeTaskBuilder<ActionTask> IntegrationTestTask { get;set; }
    public CakeTaskBuilder<ActionTask> TestTask { get; set; }
    public CakeTaskBuilder<ActionTask> CleanDocumentationTask { get; set; }
    public CakeTaskBuilder<ActionTask> DeployGraphDocumentation {get; set;}
    public CakeTaskBuilder<ActionTask> PublishDocumentationTask { get; set; }
    public CakeTaskBuilder<ActionTask> PreviewDocumentationTask { get; set; }
    public CakeTaskBuilder<ActionTask> ForcePublishDocumentationTask { get; set; }
}