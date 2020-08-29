---
Order: 60
Title: Build Method
Description: Begin the execution of Cake.Recipe
---

Cake.Recipe has three different entry points.  At the end of the recipe.cake file you can either run:

```csharp
Build.Run();
```

which will execute the Cake.Recipe steps including executing MSBuild, and .Net Framework unit testing libraries such as Xunit, and NUnit.

```csharp
Build.RunDotNetCore();
```

which will execute the Cake.Recipe steps including executing `dotnet build`, and `dotnet test`.

```csharp
Build.RunNuGet();
```

which will execute the Cake.Recipe steps without any compilation steps, but rather executing only `nuget pack`.