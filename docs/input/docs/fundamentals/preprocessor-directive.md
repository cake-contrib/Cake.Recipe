---
Order: 20
Title: Pre-processor Directive
Description: Responsible for downloading the Cake.Recipe NuGet package
---

The first line of the recipe.cake file should look something like the following:

```csharp
#load nuget:?package=Cake.Recipe&version=2.0.0
```

This will instruct Cake to download the Cake.Recipe NuGet package, which includes the *.cake files which contain the core Cake.Recipe functionality, and loads them into the current Cake execution.

Cake.Recipe is published in its released state to NuGet.org, but continuous integration builds are also published to both Azure Packages and GitHub Package Repository.

As an example, to consume the latest prerelease Cake.Recipe package from Azure Packages, you could use the following pre-processor directive:

```csharp
#load nuget:https://pkgs.dev.azure.com/cake-contrib/Home/_packaging/addins/nuget/v3/index.json?package=Cake.Recipe&prerelease
```

However, this isn't strictly recommended, as you should really pin to a specific version of this package from this feed.  As an example, you would want to use something like the following:

```csharp
#load nuget:https://pkgs.dev.azure.com/cake-contrib/Home/_packaging/addins/nuget/v3/index.json?package=Cake.Recipe&version=2.0.0-alpha0338&prerelease
```