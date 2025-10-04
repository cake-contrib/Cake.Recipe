---
Order: 20
Title: Previewing documentation
Description: How to preview documentation with Cake.Recipe
---

This document describes how to preview the documentation in a local webserver using the `Cake.Recipe` script.

1. Run:
   - On Windows: `.\build.ps1 --target=preview`
   - On MacOS/Linux: `./build.sh --target=preview`.
2. Open your browser at `http:\\localhost:5080\`

:::{.alert .alert-info}
The scripts supports watching for changes and automatically rebuilds the documentation without requiring to restart the build task.
:::
