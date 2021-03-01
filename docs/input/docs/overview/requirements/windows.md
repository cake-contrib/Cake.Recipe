---
Order: 10
Title: Windows
Description: Requirements for running Cake.Recipe on Windows
---

## Required Framework Version

In order to run Cake.Recipe on a Windows machine it is necessary to have, as a minimum, .Net Framework 4.7.2. This can be installed via Chocolatey using:

```bash
choco install dotnet4.7.2
```

If you will be building .NET Core project, Cake.Recipe will automatically use .NET Core editions of tools when possible.
In these cases there is a requirement of having .NET Core 3.1 installed.

This can easily be installed through Chocolatey using:

```console
choco install dotnetcore-runtime dotnetcore-sdk
```

## Required Cake Version

As a minimum, it is recommended that Cake.Recipe should be used in conjunction with the following versions of Cake:

{.table-bordered .table-hover}

| Cake.Recipe Version | Cake Version |
| :-----------------: | :----------: |
|         1.x         |    0.32.0    |
|         2.x         |    0.38.4    |
|         3.x         |    1.0.0     |
