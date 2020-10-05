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
* You will then be prompted to specify the name of the folder where your source code resides within your repository, this defaults to `Source`.  Once entered, press enter.
* Next, you need to provide the GitHub username (or Organisation name) for where the source code resides.  Once entered, press enter.
* Followed by the name of the repository where the code resides.  Once entered, press enter.
* A new recipe.cake file will be created with the default contents

The typical contents of this file are explained in more detail in the [fundamentals section](../fundamentals/recipe-cake).
