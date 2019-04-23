---
Order: 21
Title: How to publish documentation
---

This document describes how to publish the documentation to the configured branch using the `Cake.Recipe` script.

:::{.alert .alert-info}
Please note that for every change done in the `BuildParameters.WyamPublishDirectoryPath` automatically triggers a deployment as part of the build.
Manually publishing the documentation is only required if there are changes in the source code (Method signatures, namespaces, XML comments, ...).
:::

1. Make sure that you have the following environment variables set in your local development environment:
   * [WYAM_ACCESS_TOKEN](../fundamentals/environment-variables#wyam_access_token)
   * [WYAM_DEPLOY_BRANCH](../fundamentals/environment-variables#wyam_deploy_branch)
   * [WYAM_DEPLOY_REMOTE](../fundamentals/environment-variables#wyam_deploy_remote)
2. Run `.\build.ps1 -target publishdocs`.