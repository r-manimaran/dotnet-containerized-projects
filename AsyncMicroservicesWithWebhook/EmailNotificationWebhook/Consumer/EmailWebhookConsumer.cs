using MassTransit;
using Shared.Contracts.DTOs;

namespace EmailNotificationWebhook.Consumer;

public class EmailWebhookConsumer : IConsumer<EmailDTO>
{
    private readonly HttpClient _client;

    public EmailWebhookConsumer(HttpClient client)
    {
        _client = client;
    }
    public async Task Consume(ConsumeContext<EmailDTO> context)
    {
        var result = await _client.PostAsJsonAsync(" http://localhost:5144/email-webhook",
            new EmailDTO(context.Message.Subject, context.Message.Content));
        result.EnsureSuccessStatusCode();
    }
}
