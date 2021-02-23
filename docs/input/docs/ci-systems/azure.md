---
Order: 20
Title: Azure Pipelines
Description: Running on Azure Pipelines
---

## Specifics

* Trigger: Branches

  When no branches are specified in the trigger sections, the build *should* trigger for pushes to all branches.
  However, we have seen different behavior and hence suggest to specify branches explicitly.

* Trigger: Tags

  When no tags are specified in the trigger section the default is to not trigger on tags being pushed.

## Example Config

```yaml
pool:
  vmImage: 'windows-latest'

trigger:
  branches:
    include:
    - '*'
  tags:
    include:
    - '*'

pr:
  branches:
    include:
    - '*'

steps:
- task: Cache@2
  inputs:
    key: '"$(Agent.OS)" | recipe.cake'
    path: 'tools'
- task: Cake@2
  inputs:
    script: 'recipe.cake'
    target: 'CI'
    verbosity: 'Normal'
    Bootstrap: true
    Version: '0.38.5'
```
