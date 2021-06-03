///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

Action<string, string[], Action> RequireToolNotRegistered = (tool, toolNames, action) => {
    var toolsFolder = Context.FileSystem.GetDirectory(
        Context.Configuration.GetToolPath(Context.Environment.WorkingDirectory, Context.Environment));
    bool found = false;

    Context.Verbose("Searching for tool {0} in tools: {1}", string.Join("|", toolNames), toolsFolder.Path);
    foreach (var name in toolNames)
    {
        if (found)
        {
            break;
        }
        var path = Context.GetFiles(toolsFolder.Path + $"/**/{name}").FirstOrDefault();
        found = path != null;
    }

    if (!found)
    {
        Context.Verbose("Tool not found. Requiring '{0}'", tool);
        RequireTool(tool, action);
    }
    else
    {
        action();
    }
};

Action<string, Action> RequireTool = (tool, action) => {
    var script = MakeAbsolute(File(string.Format("./{0}.cake", Guid.NewGuid())));
    try
    {
        var arguments = new Dictionary<string, string>();

        if (BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if (BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        System.IO.File.WriteAllText(script.FullPath, tool);
        CakeExecuteScript(script,
            new CakeSettings
            {
                Arguments = arguments
            });
    }
    finally
    {
        if (FileExists(script))
        {
            DeleteFile(script);
        }
    }

    action();
};
