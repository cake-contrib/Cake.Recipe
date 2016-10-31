///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToSlackChannel(string message)
{
    try
    {
        Information("Sending message to Slack...");

        if(string.IsNullOrEmpty(parameters.Slack.Token)) {
            throw new InvalidOperationException("Could not resolve Slack Token.");
        }

        if(string.IsNullOrEmpty(parameters.Slack.Channel)) {
            throw new InvalidOperationException("Could not resolve Slack Channel.");
        }

        var postMessageResult = Slack.Chat.PostMessage(
                    token: parameters.Slack.Token,
                    channel: parameters.Slack.Channel,
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