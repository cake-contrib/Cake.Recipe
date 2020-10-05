---
Order: 6
Title: Conventions
Description: Some notes about the conventions used by Cake.Recipe
---

As set out in the introduction [Cake.Recipe is a set of convention based build scripts](../). The conventions are important, as they determine the defaults. But they are not set in stone and they can be changed in several ways

* Using [BuildParameters.SetParameters](../fundamentals/set-parameters), to change any of the parameters

* Setting [environment](../fundamentals/environment-variables) Variables

  So, what are these conventions?

* Folder Structure

  The description below is not to focus on the placement of files, but to show where the defaults are, all of these are modifiable.

  * $
    * src
      * `title`.sln
      * `title`.nuspec
    * tests, should create `.Tests` e.g. title.Tests.dll, title.integration.Tests.dll
    * build.ps1
    * build.cake

  Your cake file may specify as follows:

  ```csharp
  BuildParameters.SetParameters(context: Context,
                          sourceDirectoryPath: "./src",
                          title: `"title"`);
  ```

* Building class library

  When you are writing a reusable library, you are going to want to publish your code as a NuGet package for reuse. Whether you are writing .Net Framework based or .Net Standard based code, the recipe's intent will be to build your code, package using your nuspec file and publish. Best practices demand that your tests succeed and you meet your basic code quality criteria. All of these are turned on by default, may be turned off in the above `SetParameters` argument list.

  * To build a .Net Core package, the command `Build.RunDotNetCore()`
  * To build a .Net framework package, the command `Build.Run()` is followed with `Build.RunNuGet()`, to do a NuGet package and publish.

* Building MSI

  When you are writing a component that delivers a MSI package to your customer and NuGet publish a reusable library is not your goal, you will tend to write custom tasks to perform the MSI creation tasks after the default `Build.Run()`. Common tasks to sign your executable on the build infrastructure, sign your MSI and finally deliver it to consumers will be your custom tasks, an [example](https://github.com/chocolatey/ChocolateyGUI/blob/develop/recipe.cake)

* Diagnostic debugging

  * A common practice and a good way to learn when you begin is by printing the current calculated parameters influencing your build. To do so, add the following to your cake file
`BuildParameters.PrintParameters(Context);`
  * Another common practice when starting a new project is to execute your cake script with the diagnostic flags of cake.exe, e.g.
["--verbosity=diagnostic"](https://cakebuild.net/docs/cli/usage)
