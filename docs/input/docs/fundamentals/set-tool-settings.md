---
Order: 50
Title: SetToolSettings Method
Description: Override the default tool settings used by Cake.Recipe
---

Cake.Recipe makes use of a number of different tools, for example InspectCode, MSBuild, OpenCover, etc.  While Cake.Recipe attempts to provide sensible defaults for all of these tools, there are times when it is necessary to override these for specific project requirements.

This can be done by calling the `SetToolSettings` method.

As an example, you could override some tool settings using the following:

```csharp
ToolSettings.SetToolSettings(context: Context,
                            BuildParameters.RootDirectoryPath + "/Source/Cake.Example.Tests/*.cs",
                            BuildParameters.RootDirectoryPath + "/Source/Cake.Example/**/*.AssemblyInfo.cs" },
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* ",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");
```

## Default Tool Setting Parameters

The `SetToolSettings` method uses the concept of optional parameters, in fact, all but one of the parameters to the `SetToolSettings` method are optional.  To override a specific tool setting, you need to use a named parameter.  The following is a list of all the named parameters that can be used on the method.

### testCoverageFilter

This is used in conjunction with both OpenCover and Coverlet.  It controls which assemblies/namespaces should be included within the Unit Test coverage.

Type: `string`

Default Value:

```csharp
string.Format("+[{0}*]* -[*.Tests]*", BuildParameters.Title)
```

### testCoverageExcludeByAttribute

This is used in conjunction with both OpenCover and Coverlet.  It controls which test should be excluded from coverage based on what attributes are assigned to the tests.

Type: `string`

Default Value:

```csharp
"*.ExcludeFromCodeCoverage*"
```

### testCoverageExcludeByFile

This is used in conjunction with both OpenCover and Coverlet.  It controls which tests should be excluded based on file name.

Type: `string`

Default Value:

```csharp
"*/*Designer.cs;*/*.g.cs;*/*.g.i.cs"
```

### buildPlatformTarget

This is passed into the execution of MSBuild when doing a full .NET Framework build.

Type: `PlatformTarget?`

Default Value:

```csharp
PlatformTarget.MSIL
```

### buildMSBuildToolVersion

This is passed into the execution of MSBuild when doing a full .NET Framework build.

Type: `MSBuildToolVersion`

Default Value:

```csharp
MSBuildToolVersion.Default
```

### maxCpuCount

This is passed into the execution of MSBuild when doing a full .NET Framework build.

Type: `int?`

Default Value: 0

### targetFrameworkPathOverride

This is passed into the execution of any .NET build operation.

Type: `DirectoryPath`

Depending on what sort of build Cake.Recipe is executing, the default value changes.

If `BuildParameters.ShouldUseTargetFrameworkPath` is true, and the `targetFrameworkPathOverride` is null, and it is a .NET build, then the default value is:

```csharp
var path = context.Tools.Resolve("mono").GetDirectory();
path = path.Combine("../lib/mono/4.5");
TargetFrameworkPathOverride = path.FullPath + "/"
```

If `BuildParameters.ShouldUseTargetFrameworkPath` is true, and the `targetFrameworkPathOverride` is null, and it is _NOT_ a .NET build, then the default value is:

```csharp
TargetFrameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/"
```

If a value for `targetFrameworkPathOverride` is provided, then the default value is simply the FullPath of this DirectoryPath.

```csharp
TargetFrameworkPathOverride = targetFrameworkPathOverride?.FullPath;
```

### skipDuplicatesPackages

This is used to tell NuGet to ignore issues with trying to push package versions when the version already exist upstream, instead of failing the build.

Type: `bool`

Default Value: `false`
