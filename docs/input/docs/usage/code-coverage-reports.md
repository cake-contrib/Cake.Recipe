---
Order: 30
Title: Code coverage reports
Description: How to publish code coverage reports with Cake.Recipe
---

This document describes known issues pushing code coverage reports during Cake.Recipe builds.

## Coveralls Reports

If your project target is `netcoreapp` or `netstandard` and you are not seeing coveralls reports.  You may be seeing a step in your build server logs like the the one below.

``` powershell
========================================
Upload-Coveralls-Report
========================================
No coverage statistics files.
```

OpenCover doesn't support the [portable debug type](https://github.com/dotnet/core/blob/e6049bb60307d987e044c39a106e0d6cf98857a3/Documentation/diagnostics/portable_pdb.md), which is the default of a .NET Core project. The resolution is to change or add a `DebugType` node in your `netcoreapp` test project and **all** projects it covers.  Acceptable options are `full` or `pdbonly`.  After adding one of the following xml tags to each project in your solution that tests, or is tested by a `netstandard` or `netcoreapp` project, OpenCover will produce a valid coverage file and upload to coveralls.

``` xml
  <PropertyGroup>
    <DebugType>full</DebugType>
  </PropertyGroup>
```

or

``` xml
  <PropertyGroup>
    <DebugType>pdbonly</DebugType>
  </PropertyGroup>
```
