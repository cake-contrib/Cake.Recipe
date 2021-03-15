///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToGitterRoom(string message)
{
    try
    {
        Information("Sending message to Gitter...");

        var postMessageResult = Gitter.Chat.PostMessage(
                    message: message,
                    messageSettings: new GitterChatMessageSettings { Token = BuildParameters.Gitter.Token, RoomId = BuildParameters.Gitter.RoomId}
            );

        if (postMessageResult.Ok)
        {
            Information("Message {0} succcessfully sent", postMessageResult.TimeStamp);
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
