---
Order: 30
Title: GitHub Actions
Description: Building with GitHub Actions
---

## Specifics

* `Upload-Artifacts`-Task

  Uploading artifacts is currently not supported in Cake.Recipe. Please use the `actions/upload-artifacts` GitHub Action.

* `actions/checkout`

  The default for `fetch-depth` is `1` - this currently does not work with `GitVersion`. Set `fetch-depth: 0` to fetch all history for all branches.

## Example Config

```yaml
name: Build

on:
  push:

jobs:
  build:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v2.2.0
        with:
          fetch-depth: 0

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
          cake-version: 0.38.4
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
