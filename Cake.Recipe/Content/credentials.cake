public class GitHubCredentials
{
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string Token { get; private set; }

    public GitHubCredentials(string userName, string password, string token)
    {
        UserName = userName;
        Password = password;
        Token = token;
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

public class EmailCredentials
{
    public string SmtpHost { get; private set; }
    public int Port { get; private set; }
    public bool EnableSsl { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }

    public EmailCredentials(string smtpHost, int port, bool enableSsl, string username, string password)
    {
        SmtpHost = smtpHost;
        Port = port;
        EnableSsl = enableSsl;
        Username = username;
        Password = password;
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

public class PackageSourceCredentials
{
    public string ApiKey { get; private set; }
    public string User { get; private set; }
    public string Password { get; private set; }

    public PackageSourceCredentials(string apiKey, string user, string password)
    {
        ApiKey = apiKey;
        User = user;
        Password = password;
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

public class CodecovCredentials : CoverallsCredentials
{
    public CodecovCredentials(string repoToken)
        : base(repoToken) { }
}

public class CoverallsCredentials
{
    public string RepoToken { get; private set; }

    public CoverallsCredentials(string repoToken)
    {
        RepoToken = repoToken;
    }
}

public class TransifexCredentials : AppVeyorCredentials
{
    public bool HasCredentials
    {
        get { return !string.IsNullOrEmpty(ApiToken); }
    }

    public TransifexCredentials(string apiToken)
        : base(apiToken)
    {
    }
}

public class WyamCredentials
{
    public string AccessToken { get; private set; }
    public string DeployRemote { get; private set; }
    public string DeployBranch { get; private set; }

    public WyamCredentials(string accessToken, string deployRemote, string deployBranch)
    {
        AccessToken = accessToken;
        DeployRemote = deployRemote;
        DeployBranch = deployBranch;
    }
}

public static GitHubCredentials GetGitHubCredentials(ICakeContext context)
{
    return new GitHubCredentials(
        context.EnvironmentVariable(Environment.GithubUserNameVariable),
        context.EnvironmentVariable(Environment.GithubPasswordVariable),
        context.EnvironmentVariable(Environment.GithubTokenVariable));
}

public static EmailCredentials GetEmailCredentials(ICakeContext context)
{
    return new EmailCredentials(
        context.EnvironmentVariable(Environment.EmailSmtpHost),
        int.Parse(context.EnvironmentVariable(Environment.EmailPort) ?? "0"),
        bool.Parse(context.EnvironmentVariable(Environment.EmailEnableSsl) ?? "false"),
        context.EnvironmentVariable(Environment.EmailUserName),
        context.EnvironmentVariable(Environment.EmailPassword));
}

public static MicrosoftTeamsCredentials GetMicrosoftTeamsCredentials(ICakeContext context)
{
    return new MicrosoftTeamsCredentials(
        context.EnvironmentVariable(Environment.MicrosoftTeamsWebHookUrlVariable));
}

public static GitterCredentials GetGitterCredentials(ICakeContext context)
{
    return new GitterCredentials(
        context.EnvironmentVariable(Environment.GitterTokenVariable),
        context.EnvironmentVariable(Environment.GitterRoomIdVariable));
}

public static SlackCredentials GetSlackCredentials(ICakeContext context)
{
    return new SlackCredentials(
        context.EnvironmentVariable(Environment.SlackTokenVariable),
        context.EnvironmentVariable(Environment.SlackChannelVariable));
}

public static TwitterCredentials GetTwitterCredentials(ICakeContext context)
{
    return new TwitterCredentials(
        context.EnvironmentVariable(Environment.TwitterConsumerKeyVariable),
        context.EnvironmentVariable(Environment.TwitterConsumerSecretVariable),
        context.EnvironmentVariable(Environment.TwitterAccessTokenVariable),
        context.EnvironmentVariable(Environment.TwitterAccessTokenSecretVariable));
}

public static AppVeyorCredentials GetAppVeyorCredentials(ICakeContext context)
{
    return new AppVeyorCredentials(
        context.EnvironmentVariable(Environment.AppVeyorApiTokenVariable));
}

public static CodecovCredentials GetCodecovCredentials(ICakeContext context)
{
    return new CodecovCredentials(
        context.EnvironmentVariable(Environment.CodecovRepoTokenVariable));
}

public static CoverallsCredentials GetCoverallsCredentials(ICakeContext context)
{
    return new CoverallsCredentials(
        context.EnvironmentVariable(Environment.CoverallsRepoTokenVariable));
}

public static TransifexCredentials GetTransifexCredentials(ICakeContext context)
{
    return new TransifexCredentials(
        context.EnvironmentVariable(Environment.TransifexApiTokenVariable)
    );
}

public static WyamCredentials GetWyamCredentials(ICakeContext context)
{
    return new WyamCredentials(
        context.EnvironmentVariable(Environment.WyamAccessTokenVariable),
        context.EnvironmentVariable(Environment.WyamDeployRemoteVariable),
        context.EnvironmentVariable(Environment.WyamDeployBranchVariable));
}
