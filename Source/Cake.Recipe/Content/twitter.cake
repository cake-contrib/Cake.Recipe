///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToTwitter(string message)
{
    try
    {
        Information("Sending message to Twitter...");

        TwitterSendTweet(BuildParameters.Twitter.ConsumerKey,
                         BuildParameters.Twitter.ConsumerSecret,
                         BuildParameters.Twitter.AccessToken,
                         BuildParameters.Twitter.AccessTokenSecret,
                         message);

        Information("Message successfully sent.");
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
