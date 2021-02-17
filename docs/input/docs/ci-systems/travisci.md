---
Order: 50
Title: Travis CI
Description: Running on Travis CI
---

## Specifics

* `Upload-Artifacts`-Task

  Travis CI does not have artifacts storage therefore uploading artifacts is not supported in Cake.Recipe. 

* `actions/checkout`

  The default for `fetch-depth` currently does not work with `GitVersion`. Set `depth: false` under `git:` to fetch all history for all branches.

## Example Config

*The example shows a build-configuration for Travis CI, using windows. 
Keep in mind using windows in Travis CI is currently an [early release](https://docs.travis-ci.com/user/reference/windows)
and not everything is fully supported.*

```yaml
language: csharp
os: windows
mono: none
dotnet: none

cache:
  directories:
    - tools

git:
  depth: false

before_install:
  - choco install dotnetcore-sdk -version 3.1.402
  - powershell Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope LocalMachine

script:
  - powershell ./build.ps1
```