public void RunOpenCover(ICakeContext context, Action<ICakeContext> toolAction, bool registerUser = false)
{
    RequireTool(ToolSettings.OpenCoverTool, () =>
    {
        context.OpenCover(toolAction,
            BuildParameters.Paths.Files.TestCoverageOutputFilePath,
            new OpenCoverSettings
            {
                OldStyle = true,
                MergeOutput = context.FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath),
                ReturnTargetCodeOffset = 0,
                Register = registerUser ? "user" : null
            }
                .WithFilter(ToolSettings.TestCoverageFilter)
                .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
                .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile));
    });
}
