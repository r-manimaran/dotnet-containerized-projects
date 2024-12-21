using Shared.Contracts.DTOs;

namespace EmailNotificationWebhook.Services;

public interface IEmailService
{
    string SendEmail(EmailDTO email);
}
