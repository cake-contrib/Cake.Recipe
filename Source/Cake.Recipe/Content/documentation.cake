///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.CleanDocumentationTask = Task("Clean-Documentation")
    .Does(() =>
{
    Information("This task is intentionally left empty until a replacement tool is selected.");
});

BuildParameters.Tasks.PublishDocumentationTask = Task("Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => BuildParameters.ShouldGenerateDocumentation, "Documentation has been disabled")
    .Does(() =>
    {
        Information("This task is intentionally left empty until a replacement tool is selected.");
    }
)
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.PreviewDocumentationTask = Task("Preview-Documentation")
    .Does(() =>
    {
        Information("This task is intentionally left empty until a replacement tool is selected.");
    }
);

BuildParameters.Tasks.ForcePublishDocumentationTask = Task("Force-Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .Does(() => {
        Information("This task is intentionally left empty until a replacement tool is selected.");
    }
);