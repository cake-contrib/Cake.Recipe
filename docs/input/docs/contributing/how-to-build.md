---
Order: 20
Title: How to build Cake.Recipe
Description: Instructions how to build Cake.Recipe.
---
To build this we are using Cake.

On Windows PowerShell run:

```powershell
./build.ps1
```

On OSX/Linux run:

```bash
./build.sh
```

:::{.alert .alert-warning}
Cake.Recipe builds itself using the latest source files within the repository.  It does this by doing some additional setup within the bootstrapper files.  As such, you shouldn't run the recipe.cake file directly, as the build will not work correctly.
:::