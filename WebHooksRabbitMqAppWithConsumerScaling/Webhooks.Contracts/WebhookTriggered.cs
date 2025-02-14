namespace Webhooks.Contracts;

public sealed record WebhookTriggered(Guid SubscriptionId, string EventType, string WebhookUrl, object Data);
