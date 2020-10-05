---
Order: 20
Title: How to build Cake.Recipe
Description: Instructions on how to build Cake.Recipe
---

To build Cake.Recipe, we make use [Cake](https://cakebuild.net/).

On Windows, open a PowerShell window and run:

```powershell
./build.ps1
```

On OSX/Linux, open a terminal window and run:

```bash
./build.sh
```

:::{.alert .alert-warning}
Cake.Recipe builds itself using the latest source files within the repository. It does this by doing some additional setup within the bootstrapper files. As such, you shouldn't run the recipe.cake file directly, as the build will not work correctly.
:::

## Requirements

To build Cake.Recipe you will need to meet the same requirements as using Cake.Recipe.

These requirements can be seen [here](../overview/requirements/).
