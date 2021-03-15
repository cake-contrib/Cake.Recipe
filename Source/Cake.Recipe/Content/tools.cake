///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

Action<string, string[], Action> RequireToolNotRegistered = (tool, toolNames, action) => {
    bool found = false;

    foreach (var name in toolNames)
    {
        if (found)
        {
            break;
        }
        var path = Context.Tools.Resolve(name);
        found = path != null;
    }

    if (!found)
    {
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
