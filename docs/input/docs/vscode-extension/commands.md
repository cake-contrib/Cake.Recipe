---
Order: 20
Title: Commands
Description: Information about the commands provided by the extension
---

The Cake.Recipe VSCode extension exposes two commands, both of which can be used to create a default recipe.cake for starting to use either Cake.Recipe or Cake.VsCode.Recipe.

To make use of these, do the following:

* Open VSCode
* Open the folder that you want to use as your workspace
* Open the command palette and type Cake.Recipe
* You should see two options:
  * `Cake.Recipe: Add default recipe.cake file`
  * `Cake.VsCode.Recipe: Add default recipe.cake file`
* Select which one you need for the current project
* You will be prompted to enter the name that you want to give the file, which defaults to `recipe.cake`
* Press enter once you have the name that you want
* A new recipe.cake file will be created with the default contents

The typical contents of this file are explained in more detail in the [fundamentals section](../fundamentals/recipe-cake).
