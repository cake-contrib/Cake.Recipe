---
Order: 20
Title: Publishing Documentation from GitHub Actions
Description: Fails when running on a Windows Operating System
---

There is a known issue that occurs when running Cake.Recipe to publish documentation on GitHub Actions when using a Windows operating system.

The error appears after `KuduSync.Net` and looks like:

```
Error: Could not find a part of the path '[path-to-project]\BuildArtifacts\temp\_PublishedDocumentation\[date-and-time]\_git2_[some-hash]'.
```

The solution to this problem currently, is to not run on Windows operating system.
