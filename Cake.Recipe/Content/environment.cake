public static class Environment
{
    public static string GithubUserNameVariable { get; private set; }
    public static string GithubPasswordVariable { get; private set; }
    public static string GithubTokenVariable { get; private set; }
    public static string MyGetApiKeyVariable { get; private set; }
    public static string MyGetSourceUrlVariable { get; private set; }
    public static string MyGetUserVariable { get; private set; }
    public static string MyGetPasswordVariable { get; private set; }
    public static string NuGetApiKeyVariable { get; private set; }
    public static string NuGetSourceUrlVariable { get; private set; }
    public static string ChocolateyApiKeyVariable { get; private set; }
    public static string ChocolateySourceUrlVariable { get; private set; }
    public static string GitterTokenVariable { get; private set; }
    public static string GitterRoomIdVariable { get; private set; }
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
    public static string EmailSenderName { get; private set; }
    public static string EmailSenderAddress { get; private set; }
    public static string AppVeyorApiTokenVariable { get; private set; }
    public static string CodecovRepoTokenVariable { get; private set; }
    public static string CoverallsRepoTokenVariable { get; private set; }
    public static string MicrosoftTeamsWebHookUrlVariable { get; private set; }
    public static string TransifexApiTokenVariable { get; private set; }
    public static string WyamAccessTokenVariable { get; private set; }
    public static string WyamDeployRemoteVariable { get; private set; }
    public static string WyamDeployBranchVariable { get; private set; }

    public static void SetVariableNames(
        string githubUserNameVariable = null,
        string githubPasswordVariable = null,
        string githubTokenVariable = null,
        string myGetApiKeyVariable = null,
        string myGetSourceUrlVariable = null,
        string myGetUserVariable = null,
        string myGetPasswordVariable = null,
        string nuGetApiKeyVariable = null,
        string nuGetSourceUrlVariable = null,
        string chocolateyApiKeyVariable = null,
        string chocolateySourceUrlVariable = null,
        string gitterTokenVariable = null,
        string gitterRoomIdVariable = null,
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
        string emailSenderName = null,
        string emailSenderAddress = null,
        string appVeyorApiTokenVariable = null,
        string codecovRepoTokenVariable = null,
        string coverallsRepoTokenVariable = null,
        string microsoftTeamsWebHookUrlVariable = null,
        string transifexApiTokenVariable = null,
        string wyamAccessTokenVariable = null,
        string wyamDeployRemoteVariable = null,
        string wyamDeployBranchVariable = null)
    {
        GithubUserNameVariable = githubUserNameVariable ?? "GITHUB_USERNAME";
        GithubPasswordVariable = githubPasswordVariable ?? "GITHUB_PASSWORD";
        GithubTokenVariable = githubTokenVariable ?? "GITHUB_TOKEN";
        MyGetApiKeyVariable = myGetApiKeyVariable ?? "MYGET_API_KEY";
        MyGetSourceUrlVariable = myGetSourceUrlVariable ?? "MYGET_SOURCE";
        MyGetUserVariable = myGetUserVariable ?? "MYGET_USER";
        MyGetPasswordVariable = myGetPasswordVariable ?? "MYGET_PASSWORD";
        NuGetApiKeyVariable = nuGetApiKeyVariable ?? "NUGET_API_KEY";
        NuGetSourceUrlVariable = nuGetSourceUrlVariable ?? "NUGET_SOURCE";
        ChocolateyApiKeyVariable = chocolateyApiKeyVariable ?? "CHOCOLATEY_API_KEY";
        ChocolateySourceUrlVariable = chocolateySourceUrlVariable ?? "CHOCOLATEY_SOURCE";
        GitterTokenVariable = gitterTokenVariable ?? "GITTER_TOKEN";
        GitterRoomIdVariable = gitterRoomIdVariable ?? "GITTER_ROOM_ID";
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
        WyamAccessTokenVariable = wyamAccessTokenVariable ?? "WYAM_ACCESS_TOKEN";
        WyamDeployRemoteVariable = wyamDeployRemoteVariable ?? "WYAM_DEPLOY_REMOTE";
        WyamDeployBranchVariable = wyamDeployBranchVariable ?? "WYAM_DEPLOY_BRANCH";
    }
}
