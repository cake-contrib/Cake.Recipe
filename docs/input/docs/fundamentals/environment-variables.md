---
Order: 70
Title: Environment Variables
Description: A list of all the environment variables used by Cake.Recipe
---

# Using Environment Variables in Cake.Recipe

The various tasks within Cake.Recipe are driven, in part, by whether the correct environment variables exist on the system where the build is executing.  If they don't exist, it may mean that certain tasks are skipped, or portions of some tasks are skipped.  This will be communicated either through marking a task as skipped, or providing a warning during a task execution.

The following are a list of the default environment variable names that are looked for during the build.

:::{.alert .alert-info}
**NOTE:**

If required, the name of the environment variables can be modified to fit into your existing system/architecture by using the [SetVariableNames](./set-variable-names) method.
:::

## GitHub

Using GitReleaseManager, Cake.Recipe makes it possible to create releases, add assets to the release, and also to close a milestone.  This is made possible using either the combination of username/password, or if using 2FA a token.

:::{.alert .alert-warning}
**WARNING:**

A token will be checked for first, and if not provided, the username and password will be checked for.  Usage of the username/password combination will also being removed in a future version of Cake.Recipe, as this is being removed within the GitHub API itself.
:::

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, the control variable [shouldPublishToGitHub](./set-parameters#shouldPublishToGitHub) also needs to be set to true.
:::

### GITHUB_PAT

The Personal Access Token of the GitHub account to be used.

:::{.alert .alert-info}
**NOTE:**

If `GITHUB_PAT` is not set, the alternatives of `GH_TOKEN` and `GITHUB_TOKEN` are considered.
:::

## Slack

When a release build fails, Cake.Recipe can be configured to send out a notification (with configurable message) to a Slack Channel.  There are two required environment variables that s to be set to make this happen.  Further information about find this information can be found in the Cake.Slack [documentation](https://github.com/cake-contrib/Cake.Slack).

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, the control variable [shouldPostToSlack](./set-parameters#shouldPostToSlack) also needs to be set to true.  The default value for this parameter is true.
:::

### SLACK_TOKEN

The authentication token for the user/bot who should post the message.

### SLACK_CHANNEL

The name of the Slack Channel that the message should be sent to.

## Twitter

When a successful release build has been completed, Cake.Recipe can be configured to send out a notification (with configurable message) to Twitter.  There are four required environment variables that needs to be set to make this happen.  Further information about find this information can be found in the Cake.Twitter [documentation](https://cake-contrib.github.io/Cake.Twitter/docs/).

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, the control variable [shouldPostToTwitter](./set-parameters#shouldPostToTwitter) also needs to be set to true.  The default value for this parameter is true.
:::

### TWITTER_CONSUMER_KEY

The Consumer Key for the Twitter application that is going to be used to send the Tweet.

### TWITTER_CONSUMER_SECRET

The Consumer Secret for the Twitter application that is going to be used to send the Tweet.

### TWITTER_ACCESS_TOKEN

The Access Token for the Twitter application that is going to be used to send the Tweet.

### TWITTER_ACCESS_TOKEN_SECRET

The Access Token Secret for the Twitter application that is going to be used to send the Tweet.

## Microsoft Teams

When a successful release build has been completed, Cake.Recipe can be configured to send out a notification (with configurable message) to Microsoft Teams.  There is a single environment variable that needs to be set to make this happen.  Further information about find this information can be found in the Cake.MicrosoftTeams [documentation](https://github.com/cake-contrib/Cake.MicrosoftTeams).

:::{.alert .alert-info}
**NOTE:**

In addition to this environment variable being present, and correct, the control variable [shouldPostToMicrosoftTeams](./set-parameters#shouldPostToMicrosoftTeams) also needs to be set to true.  The default value for this parameter is false.
:::

### MICROSOFTTEAMS_WEBHOOKURL

The Web Hook Url that has been configured to allow sending messages to a particular location within Microsoft Teams.

## Email

When a successful release build has been completed, or when a release build fails, Cake.Recipe can be configured to send out a notification (with configurable message) via email.  There are  five environment variables that needs to be set to make this happen.  Further information about find this information can be found in the Cake.Email [documentation](https://cake-contrib.github.io/Cake.Email/docs/).

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, the control variable [shouldSendEmail](./set-parameters#shouldSendEmail) also needs to be set to true.  The default value for this parameter is false.
:::

### EMAIL_SMTPHOST

The SMTP Host where the email will be sent via.

### EMAIL_PORT

The port number which the SMTP server is listening on.

### EMAIL_ENABLESSL

Whether or not the SMTP server has been configured to use SSL or not.

### EMAIL_USERNAME

The username that should be used for authenticating to the SMTP server.

### EMAIL_PASSWORD

The password that should be used for authenticating to the SMTP server.

## AppVeyor

More information about what this is used for can be found in the [clean AppVeyor build cache](../usage/cleaning-cache) documentation.

:::{.alert .alert-info}
**NOTE:**

In addition to this environment variable, the [appVeyorAccountName](./set-parameters#appVeyorAccountName) and [appVeyorProjectSlug](./set-parameters#appVeyorProjectSlug) parameters need to be set correctly.

### APPVEYOR_API_TOKEN

API token for accessing AppVeyor.

## Codecov

During the build process, Cake.Recipe can publish Unit Test coverage reports to Codecov.

:::{.alert .alert-info}
**NOTE:**

In addition to this environment variable being present, and correct, the control variable [shouldRunCodecov](./set-parameters#shouldRunCodecov) also needs to be set to true.  The default value for this parameter is true.
:::

### CODECOV_REPO_TOKEN

API token for uploading coverage reports to [Codecov](https://about.codecov.io/).

## Coveralls

During the build process, Cake.Recipe can publish Unit Test coverage reports to Coveralls.

:::{.alert .alert-info}
**NOTE:**

In addition to this environment variable being present, and correct, the control variable [shouldRunCoveralls](./set-parameters#shouldRunCoveralls) also needs to be set to true.  The default value for this parameter is true.
:::

### COVERALLS_REPO_TOKEN

API token for uploading coverage reports to [Coveralls](https://coveralls.io/).

## Transifex

As part of the execution of Cake.Recipe, it is possible to download localization files from the Transifex service, and publish them along with your application.

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, there are a number of parameters that also need to be set correctly:

* [transifexEnabled](./set-parameters#transifexEnabled)
* [transifexPullMode](./set-parameters#transifexPullMode)
* [transifexPullPercentage](./set-parameters#transifexPullPercentage)
:::

### TRANSIFEX_API_TOKEN

## Wyam

During the execution of Cake.Recipe, a documentation site can also be generated (both for project source code files, and documentation files), using the Wyam Tool.  Further information about [publishing documentation](../usage/publishing-documentation) can be found in the docs.

:::{.alert .alert-info}
**NOTE:**

In addition to these environment variables being present, and correct, there are a number of parameters that also need to be set correctly:

* [wyamRootDirectoryPath](./set-parameters#wyamRootDirectoryPath)
* [wyamPublishDirectoryPath](./set-parameters#wyamPublishDirectoryPath)
* [wyamPublishDirectoryPath](./set-parameters#wyamPublishDirectoryPath)
* [wyamRecipe](./set-parameters#wyamRecipe)
* [wyamTheme](./set-parameters#wyamTheme)
* [wyamSourceFiles](./set-parameters#wyamSourceFiles)
* [webHost](./set-parameters#webHost)
* [webLinkRoot](./set-parameters#webLinkRoot)
* [webBaseEditUrl](./set-parameters#webBaseEditUrl)
* [shouldGenerateDocumentation](./set-parameters#shouldGenerateDocumentation)
* [shouldDocumentSourceFiles](./set-parameters#shouldDocumentSourceFiles)
:::

### WYAM_ACCESS_TOKEN

Access token to use to publish the Wyam documentation.

### WYAM_DEPLOY_REMOTE

URI of the remote repository where the Wyam documentation is published to.

### WYAM_DEPLOY_BRANCH

Branch into which the Wyam documentation should be published.
