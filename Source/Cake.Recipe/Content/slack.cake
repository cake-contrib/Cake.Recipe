///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToSlackChannel(string message)
{
    try
    {
        Information("Sending message to Slack...");

        var postMessageResult = Slack.Chat.PostMessage(
                    token: BuildParameters.Slack.Token,
                    channel: BuildParameters.Slack.Channel,
                    text: message
            );

        if (postMessageResult.Ok)
        {
            Information("Message {0} successfully sent", postMessageResult.TimeStamp);
        }
        else
        {
            Error("Failed to send message: {0}", postMessageResult.Error);
        }
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
