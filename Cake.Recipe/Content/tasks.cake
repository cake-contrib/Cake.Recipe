public class BuildTasks
{
    public CakeTaskBuilder DupFinderTask { get; set; }
    public CakeTaskBuilder InspectCodeTask { get; set; }
    public CakeTaskBuilder AnalyzeTask { get; set; }
    public CakeTaskBuilder PrintAppVeyorEnvironmentVariablesTask { get; set; }
    public CakeTaskBuilder UploadArtifactsTask { get; set; }
    public CakeTaskBuilder ClearAppVeyorCacheTask { get; set; }
    public CakeTaskBuilder ShowInfoTask { get; set; }
    public CakeTaskBuilder CleanTask { get; set; }
    public CakeTaskBuilder DotNetCoreCleanTask { get; set; }
    public CakeTaskBuilder RestoreTask { get; set; }
    public CakeTaskBuilder DotNetCoreRestoreTask { get; set; }
    public CakeTaskBuilder BuildTask { get; set; }
    public CakeTaskBuilder DotNetCoreBuildTask { get; set; }
    public CakeTaskBuilder PackageTask { get; set; }
    public CakeTaskBuilder DefaultTask { get; set; }
    public CakeTaskBuilder ContinuousIntegrationTask { get; set; }
    public CakeTaskBuilder ReleaseNotesTask { get; set; }
    public CakeTaskBuilder LabelsTask { get; set; }
    public CakeTaskBuilder ClearCacheTask { get; set; }
    public CakeTaskBuilder PreviewTask { get; set; }
    public CakeTaskBuilder PublishDocsTask { get; set; }
    public CakeTaskBuilder CreateChocolateyPackagesTask { get; set; }
    public CakeTaskBuilder UploadCodecovReportTask { get; set; }
    public CakeTaskBuilder UploadCoverallsReportTask { get; set; }
    public CakeTaskBuilder UploadCoverageReportTask { get; set; }
    public CakeTaskBuilder CreateReleaseNotesTask { get; set; }
    public CakeTaskBuilder ExportReleaseNotesTask { get; set; }
    public CakeTaskBuilder PublishGitHubReleaseTask { get; set; }
    public CakeTaskBuilder CreateDefaultLabelsTask { get; set; }
    public CakeTaskBuilder DotNetCorePackTask { get; set; }
    public CakeTaskBuilder CreateNuGetPackageTask { get; set; }
    public CakeTaskBuilder CreateNuGetPackagesTask { get; set; }
    public CakeTaskBuilder PublishPreReleasePackagesTask { get; set; }
    public CakeTaskBuilder PublishReleasePackagesTask { get; set; }
    public CakeTaskBuilder InstallReportGeneratorTask { get; set; }
    public CakeTaskBuilder InstallReportUnitTask { get; set; }
    public CakeTaskBuilder InstallOpenCoverTask { get; set; }
    public CakeTaskBuilder TestNUnitTask { get; set; }
    public CakeTaskBuilder TestxUnitTask { get; set; }
    public CakeTaskBuilder TestMSTestTask { get; set; }
    public CakeTaskBuilder TestVSTestTask { get; set; }
    public CakeTaskBuilder TestFixieTask { get; set; }
    public CakeTaskBuilder TransifexPullTranslations { get; set; }
    public CakeTaskBuilder TransifexPushSourceResource { get; set; }
    public CakeTaskBuilder TransifexPushTranslations { get; set; }
    public CakeTaskBuilder TransifexSetupTask { get; set; }
    public CakeTaskBuilder DotNetCoreTestTask { get; set; }
    public CakeTaskBuilder IntegrationTestTask { get;set; }
    public CakeTaskBuilder TestTask { get; set; }
    public CakeTaskBuilder CleanDocumentationTask { get; set; }
    public CakeTaskBuilder PublishDocumentationTask { get; set; }
    public CakeTaskBuilder PreviewDocumentationTask { get; set; }
    public CakeTaskBuilder ForcePublishDocumentationTask { get; set; }
    public CakeTaskBuilder ReportMessagesToCi {get; set; }
}
