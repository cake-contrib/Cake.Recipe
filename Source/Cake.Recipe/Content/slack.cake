
public class SlackReporter : FailureReporter
{
    private SlackCredentials _credentials;

    public SlackReporter(SlackCredentials credentials)
    {
        _credentials = credentials;
    }

    public override string Name { get; } = "Slack";

    public override bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.Token) &&
            !string.IsNullOrEmpty(_credentials.Channel);
    }


    public override void ReportFailure(ICakeContext context, BuildVersion _, Exception __)
    {
        try
        {
            context.Information("Sending message to Slack...");

            var postMessageResult = context.Slack().Chat.PostMessage(
                        token: _credentials.Token,
                        channel: _credentials.Channel,
                        text: "Continuous Integration Build of " + BuildParameters.Title + " just failed :-("
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

