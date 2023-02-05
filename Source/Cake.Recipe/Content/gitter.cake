public class GitterReporter : SuccessReporter
{
    private GitterCredentials _credentials;
    private string _messageTemplate;

    public GitterReporter(GitterCredentials credentials, string messageTemplate)
    {
        _credentials = credentials;
        _messageTemplate = messageTemplate;
    }

    public override string Name { get; } = "Gitter";

    public override bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.Token) &&
            !string.IsNullOrEmpty(_credentials.RoomId);
    }

    public override void ReportSuccess(ICakeContext context, BuildVersion buildVersion)
    {
        try
        {
            context.Information("Sending message to Gitter...");

            var messageArguments = BuildParameters.MessageArguments(buildVersion);
            var message = string.Format(_messageTemplate, messageArguments);

            var postMessageResult = context.Gitter().Chat.PostMessage(
                        message: message,
                        messageSettings: new GitterChatMessageSettings { Token = _credentials.Token, RoomId = _credentials.RoomId}
                );

            if (postMessageResult.Ok)
            {
                context.Information("Message {0} successfully sent", postMessageResult.TimeStamp);
            }
            else
            {
                context.Error("Failed to send message: {0}", postMessageResult.Error);
            }
        }
        catch(Exception ex)
        {
            context.Error("{0}", ex);
        }
    }
}
