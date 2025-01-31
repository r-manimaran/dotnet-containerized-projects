using Orders.Api.Models;
using Orders.Api.Repositories;

namespace Orders.Api.Services;

internal sealed class WebhookDispatcher
{
    private readonly HttpClient _httpClient;
    private readonly InMemorySubscriptionRepository _inMemorySubscriptionRepository;

    public WebhookDispatcher(HttpClient httpClient, 
                             InMemorySubscriptionRepository inMemorySubscriptionRepository)
    {
        _httpClient = httpClient;
        _inMemorySubscriptionRepository = inMemorySubscriptionRepository;
    }

    public async Task DispatchAsync(string eventType, object payload)
    {
        var subscriptions = _inMemorySubscriptionRepository.GetByEventType(eventType);
        foreach (WebhookSubscription subscription in subscriptions)
        {
            var request = new
            {
                Id = Guid.NewGuid(),
                subscription.EventType,
                SubscriptionId = subscription.Id,
                Timestamp = DateTime.UtcNow,
                Data = payload
            };
            // Send to WebhookUrl
            await _httpClient.PatchAsJsonAsync(subscription.WebhookUrl, request);
        }
    }


}
