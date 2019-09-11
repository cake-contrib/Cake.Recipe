using Cake.Email.Common;

///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendEmail(string subject, string message, string recipient)
{
    Information("Sending email...");

	// The recipient parameter can contain a single email address or a comma/semi-colon separated list of email addresses
    var recipients = recipient
		.Split(new[] { ',', ';' })
		.Select(emailAddress => new MailAddress(emailAddress))
		.ToArray();

    try
    {
        var result = Email.SendEmail(
            senderName: BuildParameters.Email.SenderName,
            senderAddress: BuildParameters.Email.SenderAddress,
            recipients: recipients,
            subject: subject,
            htmlContent: message,
            textContent: null,
            attachments: null,
            settings: new EmailSettings 
            {
                SmtpHost = BuildParameters.Email.SmtpHost,
                Port = BuildParameters.Email.Port,
                EnableSsl = BuildParameters.Email.EnableSsl,
                Username = BuildParameters.Email.Username,
                Password = BuildParameters.Email.Password
            }
        );

        if (result.Ok)
        {
            Information("Email succcessfully sent");
        }
        else
        {
            Error("Failed to send email: {0}", result.Error);
        }
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}