public class GitHubCredentials
{
    public string UserName { get; private set; }
    public string Password { get; private set; }

    public GitHubCredentials(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}

public class MicrosoftTeamsCredentials
{
    public string WebHookUrl { get; private set;}

    public MicrosoftTeamsCredentials(string webHookUrl)
    {
        WebHookUrl = webHookUrl;
    }
}

public class GitterCredentials
{
    public string Token { get; private set; }
    public string RoomId { get; private set; }

    public GitterCredentials(string token, string roomId)
    {
        Token = token;
        RoomId = roomId;
    }
}

public class SlackCredentials
{
    public string Token { get; private set; }
    public string Channel { get; private set; }

    public SlackCredentials(string token, string channel)
    {
        Token = token;
        Channel = channel;
    }
}

public class TwitterCredentials
{
    public string ConsumerKey { get; private set; }
    public string ConsumerSecret { get; private set; }
    public string AccessToken { get; private set; }
    public string AccessTokenSecret { get; private set; }

    public TwitterCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
    {
        ConsumerKey = consumerKey;
        ConsumerSecret = consumerSecret;
        AccessToken = accessToken;
        AccessTokenSecret = accessTokenSecret;
    }
}

public class MyGetCredentials
{
    public string ApiKey { get; private set; }
    public string SourceUrl { get; private set; }

    public MyGetCredentials(string apiKey, string sourceUrl)
    {
        ApiKey = apiKey;
        SourceUrl = sourceUrl;
    }
}

public class NuGetCredentials
{
    public string ApiKey { get; private set; }
    public string SourceUrl { get; private set; }

    public NuGetCredentials(string apiKey, string sourceUrl)
    {
        ApiKey = apiKey;
        SourceUrl = sourceUrl;
    }
}

public class ChocolateyCredentials
{
    public string ApiKey { get; private set; }
    public string SourceUrl { get; private set; }

    public ChocolateyCredentials(string apiKey, string sourceUrl)
    {
        ApiKey = apiKey;
        SourceUrl = sourceUrl;
    }
}

public class AppVeyorCredentials
{
    public string ApiToken { get; private set; }

    public AppVeyorCredentials(string apiToken)
    {
        ApiToken = apiToken;
    }
}

public class CoverallsCredentials
{
    public string RepoToken { get; private set; }

    public CoverallsCredentials(string repoToken)
    {
        RepoToken = repoToken;
    }
}

public static GitHubCredentials GetGitHubCredentials(ICakeContext context)
{
    return new GitHubCredentials(
        context.EnvironmentVariable(githubUserNameVariable),
        context.EnvironmentVariable(githubPasswordVariable));
}

public static MicrosoftTeamsCredentials GetMicrosoftTeamsCredentials(ICakeContext context)
{
    return new MicrosoftTeamsCredentials(
        context.EnvironmentVariable(microsoftTeamsWebHookUrl));
}

public static GitterCredentials GetGitterCredentials(ICakeContext context)
{
    return new GitterCredentials(
        context.EnvironmentVariable(gitterTokenVariable),
        context.EnvironmentVariable(gitterRoomIdVariable));
}

public static SlackCredentials GetSlackCredentials(ICakeContext context)
{
    return new SlackCredentials(
        context.EnvironmentVariable(slackTokenVariable),
        context.EnvironmentVariable(slackChannelVariable));
}

public static TwitterCredentials GetTwitterCredentials(ICakeContext context)
{
    return new TwitterCredentials(
        context.EnvironmentVariable(twitterConsumerKeyVariable),
        context.EnvironmentVariable(twitterConsumerSecretVariable),
        context.EnvironmentVariable(twitterAccessTokenVariable),
        context.EnvironmentVariable(twitterAccessTokenSecretVariable));
}

public static MyGetCredentials GetMyGetCredentials(ICakeContext context)
{
    return new MyGetCredentials(
        context.EnvironmentVariable(myGetApiKeyVariable),
        context.EnvironmentVariable(myGetSourceUrlVariable));
}

public static NuGetCredentials GetNuGetCredentials(ICakeContext context)
{
    return new NuGetCredentials(
        context.EnvironmentVariable(nuGetApiKeyVariable),
        context.EnvironmentVariable(nuGetSourceUrlVariable));
}

public static ChocolateyCredentials GetChocolateyCredentials(ICakeContext context)
{
    return new ChocolateyCredentials(
        context.EnvironmentVariable(chocolateyApiKeyVariable),
        context.EnvironmentVariable(chocolateySourceUrlVariable));
}

public static AppVeyorCredentials GetAppVeyorCredentials(ICakeContext context)
{
    return new AppVeyorCredentials(
        context.EnvironmentVariable(appVeyorApiTokenVariable));
}

public static CoverallsCredentials GetCoverallsCredentials(ICakeContext context)
{
    return new CoverallsCredentials(
        context.EnvironmentVariable(coverallsRepoTokenVariable));
}