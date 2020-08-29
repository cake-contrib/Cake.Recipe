---
Order: 10
Title: Override generating version
Description: How to replace the GitVersion version generation
---

Out of the box, Cake.Recipe uses the GitVersion tool to assert a semantic version number, based on the git history of the repository.  This asserted version number is then used throughout the remaining Cake.Recipe build, for example generating NuGet/Chocolatey packages.

If you want to replace GitVersion with another tool, this can be done by defining a new:

```csharp
Setup<BuildVersion>
```

method, which return a new instance of `BuildVersion`.

In addition, you need to define a compilation symbol named `CUSTOM_VERSIONING`, which will prevent the execution of the built in Setup method.

What follows is an example of how this can be achieved.

:::{.alert .alert-warning}
This following code snippet would have to go at the **very** start of the recipe.cake file.
:::

```csharp
#define CUSTOM_VERSIONING

Setup<BuildVersion>(context =>
{
    context.Information("Setting custom version...");
    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    return new BuildVersion
    {
        Version = "0.1.0",
        SemVersion = "0.1.0-alpha.0",
        Milestone = "0.1.0",
        CakeVersion = cakeVersion,
        InformationalVersion = "0.1.0-alpha.0+Branch.develop.Sha.528f9bf572a52f0660cbe3f4d109599eab1e9866",
        FullSemVersion = "0.1.0-alpha.0",
        AssemblySemVer = "0.1.0.o"
    };
});
```
