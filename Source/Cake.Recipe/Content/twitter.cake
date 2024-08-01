
public class TwitterReporter : SuccessReporter
{
    private TwitterCredentials _credentials;
    private string _messageTemplate;

    public TwitterReporter(TwitterCredentials credentials, string messageTemplate)
    {
        _credentials = credentials;
        _messageTemplate = messageTemplate;
    }

    public override string Name { get; } = "Twitter";

    public override bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.ConsumerKey) &&
            !string.IsNullOrEmpty(_credentials.ConsumerSecret) &&
            !string.IsNullOrEmpty(_credentials.AccessToken) &&
            !string.IsNullOrEmpty(_credentials.AccessTokenSecret);
    }


    public override void ReportSuccess(ICakeContext context, BuildVersion buildVersion)
    {
        try
        {
             context.Information("Sending message to Twitter...");

             var messageArguments = BuildParameters.MessageArguments(buildVersion);
             var message = string.Format(_messageTemplate, messageArguments);

             context.TwitterSendTweet(_credentials.ConsumerKey,
                              _credentials.ConsumerSecret,
                              _credentials.AccessToken,
                              _credentials.AccessTokenSecret,
                              message);

             context.Information("Message successfully sent.");
         }
         catch(Exception ex)
         {
             context.Error("{0}", ex);
         }
    }
}
