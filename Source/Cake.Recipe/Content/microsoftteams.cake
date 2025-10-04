public class MsTeamsReporter : SuccessReporter
{
    private MicrosoftTeamsCredentials _credentials;
    private string _messageTemplate;

    public MsTeamsReporter(MicrosoftTeamsCredentials credentials, string messageTemplate)
    {
        _credentials = credentials;
        _messageTemplate = messageTemplate;
    }

    public override string Name { get; } = "MicrosoftTeams";

    public override bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.WebHookUrl);
    }

    public override void ReportSuccess(ICakeContext context, BuildVersion buildVersion)
    {
        try
        {
            context.Information("Sending message to Microsoft Teams...");

            var messageArguments = BuildParameters.MessageArguments(buildVersion);
            var message = string.Format(_messageTemplate, messageArguments);

            context.MicrosoftTeamsPostMessage(message,
                new MicrosoftTeamsSettings {
                    IncomingWebhookUrl = _credentials.WebHookUrl
            });

        }
        catch(Exception ex)
        {
            context.Error("{0}", ex);
        }
    }
}
