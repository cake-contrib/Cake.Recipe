---
Order: 10
Title: recipe.cake file
Description: The entry point for getting started with Cake.Recipe
---

In order to start with Cake.Recipe, you need an entry point, and typically, this is via a file called recipe.cake.  In its simplest form, all that is required within this file is the following:

```csharp
#load nuget:?package=Cake.Recipe&version=2.0.0

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            title: "Cake.Example",
                            repositoryOwner: "gep13",
                            repositoryName: "Cake.Example",
                            appVeyorAccountName: "gep13");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            BuildParameters.RootDirectoryPath + "/Source/Cake.Example.Tests/*.cs",
                            BuildParameters.RootDirectoryPath + "/Source/Cake.Example/**/*.AssemblyInfo.cs" },
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* ",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

Build.RunDotNetCore();
```

The above content was generated using the [Cake.Recipe extension for VSCode](../vscode-extension), using the following input values:

* recipe.cake
* Source
* gep13
* Cake.Example

This recipe.cake file is broken up to distinct sections, some of which have more information in linked documents:

* A [pre-processor directive](./preprocessor-directive)
  * This is what is responsible for downloading the Cake.Recipe NuGet package, and loading the various script files into the current Cake execution
* A call to the [Environment.SetVariableNames](./set-variable-names) method
  * This is an opportunity to override the names for the environment variables used by Cake.Recipe
* A call to the [BuildParameters.SetParameters](./set-parameters) method
  * Cake.Recipe is driven by a set of build parameters.  Normally, the default values are all that is required, but you can override any that you would like
* A call to the [BuildParameters.PrintParameters](./print-parameters) method
  * It is really helpful, when running a build to know the value of the parameters that are being used.  Calling this method will cause them to be printed out to the build log.
* A call to the [ToolSettings.SetToolSettings](./set-tool-settings) method
  * Cake.Recipe uses a number of different tools, for example, InspectCode.  When required, you can override the settings that are passed to these tools.  Cake.Recipe attempts to provide sensible defaults for these tools.
* A call to one of the three available [Build](./build) methods
  * Cake.Recipe has three different build modes.  .NET Framework, .NET Core and NuGet.  Depending on what you are doing, you call the required one here.  This method is what causes the actual build to execute.
