---
Order: 10
Title: Posix
Description: Requirements for running Cake.Recipe on Posix
---

## Required Framework Version

In order to run Cake.Recipe on a Posix machine (i.e. Mac or Linux) it is necessary to have, as a minimum, mono 5.16.x.
If you will be building .NET Core project, Cake.Recipe will automatically use .NET Core editions of tools when possible.
In these cases there is a requirement of having .NET Core 3.1 installed.

:::::

<ul class="nav nav-tabs" role="tablist">
    <li role="presentation" class="active">
        <a href="#mac-osx" role="tab" data-toggle="tab">Mac OSX</a>
    </li>
    <li role="presentation">
        <a href="#ubuntu" role="tab" data-toggle="tab">Ubuntu</a>
    </li>
    <li role="presentation">
        <a href="#arch-linux" role="tab" data-toggle="tab">Arch Linux/Manjaro</a>
    </li>
</ul>

::::{.tab-content}

:::{.tab-pane .fade .in .active #mac-osx role=tabpanel}
To install the necessary requirements when running on Mac OSX, you can use homebrew with the following command:

```console
brew cask install mono-mdk
```

For installing .NET Core on OSX, please see the [Microsoft documentation](https://docs.microsoft.com/nb-no/dotnet/core/install/macos)

:::

:::{.tab-pane .fade #ubuntu role=tabpanel}

Most Ubuntu Versions do not have required mono version available in the official repositories.

Please follow the [Mono documentation](https://www.mono-project.com/download/stable/#download-lin-ubuntu) on how to install the latest mono version.

Then call apt with the following command to install the necessary requirements:

```console
apt-get install mono-complete ca-certificates-mono
```

_NOTE: The `ca-certificates-mono` package may be omitted on modern Ubuntu versions_.

To install .NET Core on Ubuntu, please follow the [Microsoft documentation](https://docs.microsoft.com/nb-no/dotnet/core/install/macos) for how
to add the necessary repository.

After adding the repository, run the following command to install .NET Core.

```shell
apt-get install dotnet-runtime-3.1 dotnet-sdk-3.1
```

_NOTE: Optionally you may also install the `dotnet-sdk-2.1` package_.

:::

:::{.tab-pane .fade #arch-linux role=tabpanel}
To install the necessary requirements for Arch Linux and Manjaro, run the following command:

```console
pacman -S mono
```

To install .NET Core on Arch Linux, you can install the 3.1 version with the following command

```console
pacman -S dotnet-runtime dotnet-sdk
```

_NOTE: .NET Core sdk 2.1 is available in [AUR](https://aur.archlinux.org/packages/dotnet-sdk-2.1) under the name `dotnet-sdk-2.1`_.

:::

::::
:::::

## Required Cake Version

As a minimum, it is recommended that Cake.Recipe should be used in conjunction with the following versions of Cake:

{.table-bordered .table-hover}

| Cake.Recipe Version | Cake Version |
| :-----------------: | :----------: |
|         1.x         |    0.32.0    |
|         2.x         |    0.38.4    |
|         3.x         |    1.3.0     |
