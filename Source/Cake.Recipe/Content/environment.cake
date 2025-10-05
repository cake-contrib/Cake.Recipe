public static class Environment
{
    public static string GithubTokenVariable { get; private set; }
    public static string SlackTokenVariable { get; private set; }
    public static string SlackChannelVariable { get; private set; }
    public static string TwitterConsumerKeyVariable { get; private set; }
    public static string TwitterConsumerSecretVariable { get; private set; }
    public static string TwitterAccessTokenVariable { get; private set; }
    public static string TwitterAccessTokenSecretVariable { get; private set; }
    public static string EmailSmtpHost { get; private set; }
    public static string EmailPort { get; private set; }
    public static string EmailEnableSsl { get; private set; }
    public static string EmailUserName { get; private set; }
    public static string EmailPassword { get; private set; }
    public static string AppVeyorApiTokenVariable { get; private set; }
    public static string CodecovRepoTokenVariable { get; private set; }
    public static string CoverallsRepoTokenVariable { get; private set; }
    public static string MicrosoftTeamsWebHookUrlVariable { get; private set; }
    public static string TransifexApiTokenVariable { get; private set; }
    public static string MastodonAccessTokenVariable { get; private set; }
    public static string MastodonInstanceUrlVariable { get; private set; }

    public static void SetVariableNames(
        string githubTokenVariable = null,
        string slackTokenVariable = null,
        string slackChannelVariable = null,
        string twitterConsumerKeyVariable = null,
        string twitterConsumerSecretVariable = null,
        string twitterAccessTokenVariable = null,
        string twitterAccessTokenSecretVariable = null,
        string emailSmtpHost = null,
        string emailPort = null,
        string emailEnableSsl = null,
        string emailUserName = null,
        string emailPassword = null,
        string appVeyorApiTokenVariable = null,
        string codecovRepoTokenVariable = null,
        string coverallsRepoTokenVariable = null,
        string microsoftTeamsWebHookUrlVariable = null,
        string transifexApiTokenVariable = null,
        string mastodonAccessTokenVariable = null,
        string mastodonInstanceUrlVariable = null)
    {
        GithubTokenVariable = githubTokenVariable ?? "GITHUB_PAT";
        SlackTokenVariable = slackTokenVariable ?? "SLACK_TOKEN";
        SlackChannelVariable = slackChannelVariable ?? "SLACK_CHANNEL";
        TwitterConsumerKeyVariable = twitterConsumerKeyVariable ?? "TWITTER_CONSUMER_KEY";
        TwitterConsumerSecretVariable = twitterConsumerSecretVariable ?? "TWITTER_CONSUMER_SECRET";
        TwitterAccessTokenVariable = twitterAccessTokenVariable ?? "TWITTER_ACCESS_TOKEN";
        TwitterAccessTokenSecretVariable = twitterAccessTokenSecretVariable ?? "TWITTER_ACCESS_TOKEN_SECRET";
        EmailSmtpHost = emailSmtpHost ?? "EMAIL_SMTPHOST";
        EmailPort = emailPort ?? "EMAIL_PORT";
        EmailEnableSsl = emailEnableSsl ?? "EMAIL_ENABLESSL";
        EmailUserName = emailUserName ?? "EMAIL_USERNAME";
        EmailPassword = emailPassword ?? "EMAIL_PASSWORD";
        AppVeyorApiTokenVariable = appVeyorApiTokenVariable ?? "APPVEYOR_API_TOKEN";
        CodecovRepoTokenVariable = codecovRepoTokenVariable ?? "CODECOV_REPO_TOKEN";
        CoverallsRepoTokenVariable = coverallsRepoTokenVariable ?? "COVERALLS_REPO_TOKEN";
        MicrosoftTeamsWebHookUrlVariable = microsoftTeamsWebHookUrlVariable ?? "MICROSOFTTEAMS_WEBHOOKURL";
        TransifexApiTokenVariable = transifexApiTokenVariable ?? "TRANSIFEX_API_TOKEN";
        MastodonAccessTokenVariable = mastodonAccessTokenVariable ?? "MASTODON_ACCESS_TOKEN";
        MastodonInstanceUrlVariable = mastodonInstanceUrlVariable ?? "MASTODON_INSTANCE_URL";
    }
}
