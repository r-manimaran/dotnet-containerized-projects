using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Models;
using Orders.Api.Repositories;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Channels;

namespace Orders.Api.Services;

internal sealed class WebhookDispatcherUsingDB
{
    private readonly IPublishEndpoint _publishEndpoint;
    public WebhookDispatcherUsingDB(IPublishEndpoint publishEndpoint)
    {
      _publishEndpoint = publishEndpoint;
    }

    public async Task DispatchAsync<T>(string eventType, T data) where T : notnull
    {
        using Activity? activity = Activity.Current?.Source.StartActivity($"{eventType} - Dispatch Webhook");
        activity?.SetTag("EventType", eventType);
        // publish to RabbitMQ queue
        await _publishEndpoint.Publish(new WebhookDispatched(eventType, data));
    }

}
