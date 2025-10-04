---
Order: 10
Title: Running Xunit Tests when targeting .NET Framework
Description: Can cause problems when running on a non-Windows Operating System
---

There is a known issue that can occur when attempting to run Cake.Recipe (on a non Windows environment) against full .NET Framework when the solution that is being built includes an Xunit Test Project.

The error that can appear is:

```bash
[xUnit.net 00:00:00.76] XUnitTests: Catastrophic failure: System.TypeLoadException: Could not load type of field 'Xunit.Runner.VisualStudio.VsExecutionSink:recorder' (4) due to: Could not load file or assembly 'Microsoft.VisualStudio.TestPlatform.ObjectModel, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies.
```

The solution to this problem is to add the following package reference into your test project:

```xml
<PackageReference Include="Microsoft.TestPlatform.ObjectModel" Version="16.6.1" Condition="$(TargetFramework.StartsWith('net4')) AND '$(OS)' == 'Unix'" />
```

:::{.alert .alert-info}
**NOTE:**
This package reference version should match the package reference version of the `Microsoft.NET.Test.Sdk` package.
:::
