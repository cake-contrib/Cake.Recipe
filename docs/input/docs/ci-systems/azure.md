---
Order: 20
Title: Azure Pipelines
Description: Running on Azure Pipelines
---

## Specifics

None.

## Example Config

```yaml
pool:
  vmImage: 'windows-latest'

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
    Version: '0.38.4'
```
