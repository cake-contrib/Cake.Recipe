---
Order: 10
---

The various tasks within Cake.Recipe are driven, in part, by whether the correct environment variables exist on the system where the build is executing.

The following are a list of the default environment variable names that are looked for during the build.

:::{.alert .alert-info}
**NOTE:**

If required, the name of the environment variables can be modified to fit into your existing system/architecture.
:::

# GitHub

## GITHUB_USERNAME

User name of the GitHub account used to create and publish releases.

## GITHUB_PASSWORD

Password or [Personal Access Token](https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/) of the GitHub account used to create and publish releases.

## GITHUB_TOKEN

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

# Email

## EMAIL_SMTPHOST

## EMAIL_PORT

## EMAIL_ENABLESSL

## EMAIL_USERNAME

## EMAIL_PASSWORD

# AppVeyor

## APPVEYOR_API_TOKEN

API token for accessing AppVeyor. Used to [clean AppVeyor build cache](../usage/cleaning-cache).

# Codecov

## CODECOV_REPO_TOKEN

API token for uploading coverage reports to codecov.io. *This token is entirely optional for public repositories building on AppVeyor, and is recommended to only be used on private repositories*

# Coveralls

## COVERALLS_REPO_TOKEN

# Microsoft Teams

## MICROSOFTTEAMS_WEBHOOKURL

# Transifex

## TRANSIFEX_API_TOKEN

# Wyam

## WYAM_ACCESS_TOKEN

Access token to use to publish the Wyam documentation. Used to [publish documentation](../usage/publishing-documentation).

## WYAM_DEPLOY_REMOTE

URI of the remote repository where the Wyam documentation is published to. Used to [publish documentation](../usage/publishing-documentation).

## WYAM_DEPLOY_BRANCH

Branch into which the Wyam documentation should be published. Used to [publish documentation](../usage/publishing-documentation).