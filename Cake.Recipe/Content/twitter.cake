///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToTwitter(string message)
{
    try
    {
        Information("Sending message to Twitter...");

        if(string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerKey)) {
            throw new InvalidOperationException("Could not resolve Twitter ConsumerKey.");
        }

        if(string.IsNullOrEmpty(BuildParameters.Twitter.ConsumerSecret)) {
            throw new InvalidOperationException("Could not resolve Twitter ConsumerSecret.");
        }

        if(string.IsNullOrEmpty(BuildParameters.Twitter.AccessToken)) {
            throw new InvalidOperationException("Could not resolve Twitter AccessToken.");
        }

        if(string.IsNullOrEmpty(BuildParameters.Twitter.AccessTokenSecret)) {
            throw new InvalidOperationException("Could not resolve Twitter AccessTokenSecret.");
        }

        TwitterSendTweet(BuildParameters.Twitter.ConsumerKey, BuildParameters.Twitter.ConsumerSecret, BuildParameters.Twitter.AccessToken, BuildParameters.Twitter.AccessTokenSecret, message);

        Information("Message succcessfully sent.");
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}