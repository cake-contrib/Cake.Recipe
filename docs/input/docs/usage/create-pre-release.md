---
Order: 11
Title: Creating a pre-release
Description: How to create a pre release (unstable release) using Cake.Recipe
---

This document describes the suggested steps to release a new pre-release version using the `Cake.Recipe` script.

:::{.alert .alert-info}
Please note that the following steps assume you're using `Cake.Recipe` on GitHub and with a GitFlow workflow.
Both are not requirements though and you can adopt the steps to other environments.

The process of creating pre-release versions are almost identical to the process of creating stable releases
which are documented [here](creating-release){.alert-link}.
:::

1. Create the branch where you want the drafted release notes to be based off (recommended to use release/hotfix branches for beta releases).
2. Make sure that a GitHub milestone exists for this release.
3. Make sure there were issues for all changes with the appropriate labels and the correct milestone set.
4. Make sure that you have the following environment variables set in your local development environment:
   - [GITHUB_TOKEN](../fundamentals/environment-variables#github_token)
5. Create a GitHub release draft by running:
   - On Windows: `.\build.ps1 --target=releasenotes --create-pre-release`
   - On MacOS/Linux: `./build.sh --target=releasenotes --create-pre-release`
6. Check the generated release notes and make required manual changes.
7. Publish the draft release on GitHub.

The last step will tag the release and trigger another build including the publishing. The build will automatically publish the build artifacts to the GitHub release, publish to NuGet and notify about the new release through Twitter and Gitter ((by default, but it is also possbile to add notifications to [Microsoft Teams](../fundamentals/environment-variables#microsoft-teams) and [email](../fundamentals/environment-variables#email))), based on your specific settings.
