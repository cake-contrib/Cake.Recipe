///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToMicrosoftTeams(string message)
{
    try
    {
        Information("Sending message to Microsoft Teams...");

        MicrosoftTeamsPostMessage(message,
            new MicrosoftTeamsSettings {
                IncomingWebhookUrl = BuildParameters.MicrosoftTeams.WebHookUrl
        });

    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
