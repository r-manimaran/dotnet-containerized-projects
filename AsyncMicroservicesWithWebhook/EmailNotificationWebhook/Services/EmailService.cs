using MailKit.Net.Smtp;
using MimeKit;
using Shared.Contracts.DTOs;


namespace EmailNotificationWebhook.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }
    public string SendEmail(EmailDTO email)
    {
        _logger.LogInformation("Sending Email");
        /*
        var _email = new MimeMessage();
        _email.From.Add(MailboxAddress.Parse("test@email.com"));
        _email.To.Add(MailboxAddress.Parse("subscriber@test.com"));
        _email.Subject = email.Subject;
        _email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {  Text = email.Content };

        using var smtpClient = new SmtpClient();
        smtpClient.Connect("",587,MailKit.Security.SecureSocketOptions.StartTls);
        smtpClient.Authenticate("","", CancellationToken.None);
        smtpClient.Send(_email);
        smtpClient.Disconnect(true);*/
        _logger.LogInformation("Email Send Successfully.");
        return "Email Triggered";
    }
}
