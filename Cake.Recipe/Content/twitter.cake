///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin nuget:?package=Cake.Twitter&version=0.1.0

///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToTwitter(string message)
{
    try
    {
        Information("Sending message to Twitter...");

        if(string.IsNullOrEmpty(parameters.Twitter.ConsumerKey)) {
            throw new InvalidOperationException("Could not resolve Twitter ConsumerKey.");
        }

        if(string.IsNullOrEmpty(parameters.Twitter.ConsumerSecret)) {
            throw new InvalidOperationException("Could not resolve Twitter ConsumerSecret.");
        }

        if(string.IsNullOrEmpty(parameters.Twitter.AccessToken)) {
            throw new InvalidOperationException("Could not resolve Twitter AccessToken.");
        }

        if(string.IsNullOrEmpty(parameters.Twitter.AccessTokenSecret)) {
            throw new InvalidOperationException("Could not resolve Twitter AccessTokenSecret.");
        }

        TwitterSendTweet(parameters.Twitter.ConsumerKey, parameters.Twitter.ConsumerSecret, parameters.Twitter.AccessToken, parameters.Twitter.AccessTokenSecret, message);

        Information("Message succcessfully sent.");
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}