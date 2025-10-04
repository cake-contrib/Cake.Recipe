public class MastodonReporter : SuccessReporter
{
    private MastodonCredentials _credentials;
    private string _messageTemplate;

    public MastodonReporter(MastodonCredentials credentials, string messageTemplate)
    {
        _credentials = credentials;
        _messageTemplate = messageTemplate;
    }

    public override string Name { get; } = "Mastodon";

    public override bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.AccessToken) &&
            !string.IsNullOrEmpty(_credentials.InstanceUrl);
    }


    public override void ReportSuccess(ICakeContext context, BuildVersion buildVersion)
    {
        try
        {
             context.Information("Sending message to Mastodon...");

             var messageArguments = BuildParameters.MessageArguments(buildVersion);
             var message = string.Format(_messageTemplate, messageArguments);
             var idempotencyKey = Guid.NewGuid().ToString("d");

             context.MastodonSendToot(_credentials.InstanceUrl,
                              _credentials.AccessToken,
                              message,
                              idempotencyKey);

             context.Information("Message successfully sent.");
         }
         catch(Exception ex)
         {
             context.Error("{0}", ex);
         }
    }
}
