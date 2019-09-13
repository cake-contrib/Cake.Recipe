///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToMicrosoftTeams()
{
    try
    {
        Information("Sending message to Microsoft Teams...");

        MicrosoftTeamsPostMessage(BuildParameters.MicrosoftTeamsMessage,
            new MicrosoftTeamsSettings {
                IncomingWebhookUrl = BuildParameters.MicrosoftTeams.WebHookUrl
        });

    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
