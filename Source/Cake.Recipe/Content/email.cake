using Cake.Email.Common;

public class EmailReporter : ISuccessReporter, IFailureReporter
{
    private EmailCredentials _credentials;

    public EmailReporter(EmailCredentials credentials)
    {
        _credentials = credentials;
    }

    public string Name { get; } = "EMail";

    public bool CanBeUsed
    {
        get => !string.IsNullOrEmpty(_credentials.SmtpHost)
            && !string.IsNullOrEmpty(BuildParameters.EmailRecipient);
    }

    public bool ShouldBeUsed { get; set; }

    public void ReportSuccess(ICakeContext context, BuildVersion buildVersion)
    {
        var subject = $"Continuous Integration Build of {BuildParameters.Title} completed successfully";
        var messageArguments = BuildParameters.MessageArguments(buildVersion);
        var message = new StringBuilder();
        message.AppendLine(string.Format(BuildParameters.StandardMessage, messageArguments) + "<br/>");
        message.AppendLine("<br/>");
        message.AppendLine($"<strong>Name</strong>: {BuildParameters.Title}<br/>");
        message.AppendLine($"<strong>Version</strong>: {buildVersion.SemVersion}<br/>");
        message.AppendLine($"<strong>Configuration</strong>: {BuildParameters.Configuration}<br/>");
        message.AppendLine($"<strong>Target</strong>: {BuildParameters.Target}<br/>");
        message.AppendLine($"<strong>Cake version</strong>: {buildVersion.CakeVersion}<br/>");
        message.AppendLine($"<strong>Cake.Recipe version</strong>: {BuildMetaData.Version}<br/>");

        SendEmail(context, subject, message.ToString(), BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
    }

    public void ReportFailure(ICakeContext context, BuildVersion _, Exception thrownException)
    {
        var subject = $"Continuous Integration Build of {BuildParameters.Title} failed";
        var message = thrownException.ToString().Replace(System.Environment.NewLine, "<br/>");

        SendEmail(context, subject, message, BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
    }

    private void SendEmail(ICakeContext context, string subject, string message, string recipient, string senderName, string senderAddress)
    {
        context.Information("Sending email...");

        // The recipient parameter can contain a single email address or a comma/semi-colon separated list of email addresses
        var recipients = recipient
            .Split(new[] { ',', ';' }, StringSplitOptions.None)
            .Select(emailAddress => new MailAddress(emailAddress))
            .ToArray();

        try
        {
            var result = context.Email().SendEmail(
                senderName: senderName,
                senderAddress: senderAddress,
                recipients: recipients,
                subject: subject,
                htmlContent: message,
                textContent: null,
                attachments: null,
                settings: new EmailSettings
                {
                    SmtpHost = _credentials.SmtpHost,
                    Port = _credentials.Port,
                    EnableSsl = _credentials.EnableSsl,
                    Username = _credentials.Username,
                    Password = _credentials.Password
                }
            );

            if (result.Ok)
            {
                context.Information("Email successfully sent");
            }
            else
            {
                context.Error("Failed to send email: {0}", result.Error);
            }
        }
        catch(Exception ex)
        {
            context.Error("{0}", ex);
        }
    }
}
