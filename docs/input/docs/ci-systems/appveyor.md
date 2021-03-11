---
Order: 10
Title: AppVeyor
Description: Running on AppVeyor
---

## Specifics

* PR-Builds

  In organizations with large amounts of projects (like cake-contrib) it is advisable to *not* run 
  pull-requests on AppVeyor (if using the OSS-plan in AppVeyor), to
  reduce the amount of build-time.
  This change can not be done in the yaml but must be done manually in the UI.
  
  Go to AppVeyor project -> settings -> "Do not build on "Pull Request" events" to disable building on pull-requests.

## Example Config

:::{.alert .alert-info}
**NOTE:**

The following example shows a pinned virtual environment.
The current list of available environments for AppVeyor can be found in 
[the documentation](https://www.appveyor.com/docs/build-environment/#build-worker-images).

:::

```yaml
image:
  - Visual Studio 2019

test: off
build: off

build_script:
  - ps: .\build.ps1 --target=CI

cache:
  - "tools -> recipe.cake"
```
