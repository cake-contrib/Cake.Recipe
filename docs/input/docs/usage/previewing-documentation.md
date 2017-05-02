---
Order: 20
Title: How to preview documentation
---

This document describes how to preview the documentation in a local webserver using the `Cake.Recipe` script.

1. Run `.\build.ps1 -target preview`.
2. Open your browser at `http:\\localhost:5080\<BuildParameters.WebLinkRoot>`

:::{.alert .alert-info}
The scripts supports watching for changes and automatically rebuilds the documentation without requiring to restart the build task.
:::