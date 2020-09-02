---
Order: 10
Title: Posix
Description: Requirements for running Cake.Recipe on Posix
---

## Required Framework Version

In order to run Cake.Recipe on a Posix machine (i.e. Mac or Linux) it is necessary to have, as a minimum, mono 5.16.x.

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

:::

:::{.tab-pane .fade #ubuntu role=tabpanel}

Most Ubuntu Versions do not have required mono version available in the official repositories.

Please follow the [Mono documentation](https://www.mono-project.com/download/stable/#download-lin-ubuntu) on how to install the latest mono version.

Then call apt with the following command to install the necessary requirements:

```console
apt-get install mono-complete ca-certificates-mono
```

_NOTE: The `ca-certificates-mono` package may be omitted on modern Ubuntu versions_.

:::

:::{.tab-pane .fade #arch-linux role=tabpanel}
To install the necessary requirements for Arch Linux and Manjaro, run the following command:

```console
pacman -S mono
```

:::

::::
:::::

## Required Cake Version

As a minimum, it is recommended that Cake.Recipe should be used in conjunction with the following versions of Cake:

| Cake.Recipe Version | Cake Version |
| ------------------- | ------------ |
| 0.1.x               | 0.32.0       |
| 0.2.x               | 0.38.4       |
