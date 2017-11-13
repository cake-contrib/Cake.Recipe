///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToTwitter()
{
    try
    {
        Information("Sending message to Twitter...");

        TwitterSendTweet(BuildParameters.Twitter.ConsumerKey,
                         BuildParameters.Twitter.ConsumerSecret,
                         BuildParameters.Twitter.AccessToken,
                         BuildParameters.Twitter.AccessTokenSecret,
                         BuildParameters.TwitterMessage);

        Information("Message succcessfully sent.");
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}