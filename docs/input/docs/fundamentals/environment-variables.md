---
Order: 10
---

The various tasks within Cake.Recipe are driven, in part, by whether the correct environment variables exist on the system where the build is executing.

The following are a list of the default environment variable names that are looked for during the build.

**NOTE:** If required, the name of the environment variables can be modified to fit into your existing system/architecture.

# GitHub

## GITHUB_USERNAME

User name of the GitHub account used to create and publish releases.

## GITHUB_PASSWORD

Password of the GitHub account used to create and publish releases.

# MyGet

## MYGET_API_KEY

## MYGET_SOURCE

# NuGet

## NUGET_API_KEY

## NUGET_SOURCE

# Gitter

## GITTER_TOKEN

## GITTER_ROOM_ID

# Slack

## SLACK_TOKEN

## SLACK_CHANNEL

# Twitter

## TWITTER_CONSUMER_KEY

## TWITTER_CONSUMER_SECRET

## TWITTER_ACCESS_TOKEN

## TWITTER_ACCESS_TOKEN_SECRET

# Codecov

## CODECOV_REPO_TOKEN

API token for uploading coverage reports to codecov.io. *This token is entirely optional for public repositories building on AppVeyor, and is recommended to only be used on private repositories*

# Coveralls

## COVERALLS_REPO_TOKEN

# AppVeyor

## APPVEYOR_API_TOKEN

API token for accessing AppVeyor. Used to [clean AppVeyor build cache](../usage/cleaning-cache).

# Wyam

## WYAM_ACCESS_TOKEN

Access token to use to publish the Wyam documentation. Used to [publish documentation](../usage/publishing-documentation).

## WYAM_DEPLOY_REMOTE

URI of the remote repository where the Wyam documentation is published to. Used to [publish documentation](../usage/publishing-documentation).

## WYAM_DEPLOY_BRANCH

Branch into which the Wyam documentation should be published. Used to [publish documentation](../usage/publishing-documentation).