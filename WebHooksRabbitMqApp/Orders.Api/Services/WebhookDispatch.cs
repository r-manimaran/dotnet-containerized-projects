namespace Orders.Api.Services;

public sealed record WebhookDispatched (string EventType, object Data);

