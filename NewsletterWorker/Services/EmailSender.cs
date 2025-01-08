using Microsoft.Extensions.Options;
using NewsletterWorker.Configuration;
using System.Net;
using System.Net.Mail;

namespace NewsletterWorker.Services;

public class EmailSender(IOptions<EmailOptions> emailOptions)
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public void SendEmail(string recipientEmail, string? name, string subject, string content)
    {
        var fromAddress = new MailAddress(_emailOptions.SmtpEmail, _emailOptions.DisplayName);
        var toAddress = new MailAddress(recipientEmail, name);

        using var smtp = new SmtpClient
        {
            Host = _emailOptions.SmtpHost,
            Port = _emailOptions.SmtpPort,
            EnableSsl = _emailOptions.SmtpEnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailOptions.SmtpUserName, _emailOptions.SmtpPassword),
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = content,
            IsBodyHtml = true
        };

        smtp.Send(message);
    }
}
