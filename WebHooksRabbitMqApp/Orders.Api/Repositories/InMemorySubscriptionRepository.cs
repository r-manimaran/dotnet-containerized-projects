using Orders.Api.Models;

namespace Orders.Api.Repositories;

internal sealed class InMemorySubscriptionRepository
{
    private readonly ILogger<InMemorySubscriptionRepository> _logger;
    public readonly List<WebhookSubscription> _inMemorySubscriptions = new List<WebhookSubscription>();
    public InMemorySubscriptionRepository(ILogger<InMemorySubscriptionRepository> logger)
    {
        _logger = logger;
    }

    public void AddWebhookSubscription(WebhookSubscription subscription)
    {
        _logger.LogInformation("Adding new Subscription {subscription}", subscription);
        _inMemorySubscriptions.Add(subscription);
    }

    public IReadOnlyList<WebhookSubscription> GetByEventType(string eventType)
    {
        _logger.LogInformation("Returning Subscription");
        return _inMemorySubscriptions.Where(t=>t.EventType == eventType).ToList().AsReadOnly();
    }
}
