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

* Matrix build

  GitHub Actions support building the same build on multiple OS. This is called a `matrix` build.
  Testing builds on different systems is highly encouraged, therefore the example will
  show a `matrix` build.


## Example Config

:::{.alert .alert-info}
**NOTE:**

The following example shows pinned virtual environments.
The current list of available environments for GitHub actions can be found in 
[the documentation](https://docs.github.com/en/actions/using-github-hosted-runners/about-github-hosted-runners#supported-runners-and-hardware-resources).

:::


```yaml
name: Build

on:
  push:
    branches:
      - main
      - develop
      - "feature/**"
      - "release/**"
      - "hotfix/**"
    tags:
      - "*"
  pull_request:

jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ windows-2019, ubuntu-18.04, macos-10.15 ]

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
      
      - name: Build project
        uses: cake-build/cake-action@v1
        with:
          script-path: recipe.cake
          target: CI
          verbosity: Normal
          cake-version: 1.0.0
          cake-bootstrap: true

      - name: Upload Issues-Report
        uses: actions/upload-artifact@v2
        with:
          if-no-files-found: warn
          name: ${{ matrix.os }} issues
          path: BuildArtifacts/report.html

      - name: Upload Packages
        uses: actions/upload-artifact@v2
        if: runner.os == 'Windows'
        with:
          if-no-files-found: warn
          name: package
          path: BuildArtifacts/Packages/**/*
```
