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

* PR builds
  
  When building on pull requests, GitHub actions will start two builds: One on the target branch and 
  one on the source branch. This is unwanted in cases where the source and target branch originate
  in the same repository. (E.g. when doing a PR from `bugfix/GH-12` to `develop`). GitHub actions
  currently has no builtin feature to suppress this. (like AppVeyor has a setting `skip_branch_with_pr`.)
  To mimic this feature the example shows a conditional build using `if`. The source of this workaround
  can be found in the GitHub community thread 
  [*Duplicate checks on “push” and “pull_request” simultaneous event*](https://github.community/t/duplicate-checks-on-push-and-pull-request-simultaneous-event/18012)

## Example Config

```yaml
name: Build

on:
  push:
  pull_request:

jobs:
  build:
    runs-on: ${{ matrix.os }}
    if: github.event_name == 'push' || github.event.pull_request.head.repo.full_name != github.repository
    strategy:
      matrix:
        os: [ windows-latest, ubuntu-latest, macos-latest ]

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
          cake-version: 0.38.5
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
