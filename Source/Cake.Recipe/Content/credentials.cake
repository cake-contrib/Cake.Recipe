public class GitHubCredentials
{
    public string Token { get; private set; }

    public GitHubCredentials(string token)
    {
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
    public bool HasCredentials
    {
        get { return !string.IsNullOrEmpty(RepoToken); }
    }

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
    string token = null;
    // if "GithubTokenVariable" is not set, fallback to the gh-cli defaults of GH_TOKEN, GITHUB_TOKEN
    var variableNames = new[]{ Environment.GithubTokenVariable, "GH_TOKEN", "GITHUB_TOKEN" };
    foreach (var name in variableNames)
    {
        token = context.EnvironmentVariable(name);
        if(!string.IsNullOrEmpty(token))
        {
            break;
        }
    }
    return new GitHubCredentials(token);
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
    var token = context.EnvironmentVariable(Environment.CodecovRepoTokenVariable);

    if (string.IsNullOrEmpty(token))
    {
        // Fallback to attempt to check for the conventional CODECOV_TOKEN which
        // the CLI tools read automatically.
        token = context.EnvironmentVariable("CODECOV_TOKEN");
    }

    return new CodecovCredentials(token);
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
