---
Order: 30
Title: GitHub Actions
Description: Building with GitHub Actions
---

## Specifics

* `Upload-Artifacts`-Task

  Uploading artifacts is currently not supported in Cake.Recipe. Please use the `actions/upload-artifacts` GitHub Action.

* `actions/checkout`

  The default for `fetch-depth` is `1` - this currently does not work with `GitVersion`. 
  Additionally information on different tags and branches is needed on some occasions 
  (like building from a tag which does happen when releasing a new version.)

  Therefore it is required to "unshallow" the checked out repository by adding a manual 
  step: `git fetch --prune --unshallow`

## Example Config

```yaml
name: Build

on:
  push:

jobs:
  build:
    runs-on: windows-latest

    steps:
      - name: Checkout the repository 
        uses: actions/checkout@v2
      
      - name: Fetch all tags and branches
        run: git fetch --prune --unshallow

      - name: Cache Tools
        uses: actions/cache@v2
        with:
          path: tools
          key: ${{ runner.os }}-tools-${{ hashFiles('recipe.cake') }}
      
      - name: Setup .NET Core 3.1
        uses: actions/setup-dotnet@v1.5.0
        with:
          dotnet-version: 3.1.107
      
      - name: Build project
        uses: cake-build/cake-action@v1
        with:
          script-path: recipe.cake
          target: CI
          verbosity: Normal
          cake-version: 0.38.5
          cake-bootstrap: true

      - name: Upload Issues-Report
        uses: actions/upload-artifact@v2
        with:
          if-no-files-found: warn
          name: issues
          path: BuildArtifacts/report.html

      - name: Upload Packages
        uses: actions/upload-artifact@v2
        with:
          if-no-files-found: warn
          name: package
          path: BuildArtifacts/Packages/**/*
```
