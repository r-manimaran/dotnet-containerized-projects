using MassTransit;
using Orders.Api.Models;
using System.Text.Json;
using Webhooks.Contracts;
using Webhooks.Processing.Data;
namespace Webhooks.Processing.Services;

internal sealed class WebhookTriggeredConsumer : IConsumer<WebhookTriggered>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WebhookDbContext _dbContext;
    public WebhookTriggeredConsumer(IHttpClientFactory httpClientFactory, WebhookDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<WebhookTriggered> context)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var payload = new WebhookPayload
        {
            Id = Guid.NewGuid(),
            EventType = context.Message.EventType,
            SubscriptionId = context.Message.SubscriptionId,
            Timestamp = DateTime.UtcNow,
            Data = context.Message.Data
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        try
        {
            // Send to WebhookUrl
            var response = await httpClient.PatchAsJsonAsync(context.Message.WebhookUrl, payload);
            response.EnsureSuccessStatusCode();

            var attempt = new WebhookDeliveryAttempt
            {
                Id = Guid.NewGuid(),
                WebhookSubscriptionId = context.Message.SubscriptionId,
                Payload = jsonPayload,
                ResponseStatusCode = (int)response.StatusCode,
                Success = response.IsSuccessStatusCode,
                Timestamp = DateTime.UtcNow,
            };

            _dbContext.WebhookDeliveryAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            var attempt = new WebhookDeliveryAttempt
            {
                Id = Guid.NewGuid(),
                WebhookSubscriptionId = context.Message.SubscriptionId,
                Payload = jsonPayload,
                ResponseStatusCode = null,
                Success = false,
                Timestamp = DateTime.UtcNow,
            };
            _dbContext.WebhookDeliveryAttempts.Add(attempt);
            await _dbContext.SaveChangesAsync();

        }
    }
}
