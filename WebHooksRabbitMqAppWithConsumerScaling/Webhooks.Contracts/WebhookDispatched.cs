namespace Webhooks.Contracts;

public sealed record WebhookDispatched (string EventType, object Data);

