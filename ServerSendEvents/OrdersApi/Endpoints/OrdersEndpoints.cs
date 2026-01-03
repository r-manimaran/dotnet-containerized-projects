using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using System.Net.ServerSentEvents;
using System.Threading.Channels;

namespace OrdersApi.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("orders/realtime", async (ChannelReader<Order> channelReader, 
            CancellationToken cancellationToken) =>
        {
            return Results.ServerSentEvents(
                channelReader.ReadAllAsync(cancellationToken),
                "orders");            
        });

        app.MapGet("orders/realtime/with-events", (ChannelReader<Order> channelReader,
            OrderEventBuffer orderEventBuffer,
            [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
            CancellationToken cancellationToken) =>
        {
            async IAsyncEnumerable<SseItem<Order>> StreamEvents()
            {
                if(!string.IsNullOrWhiteSpace(lastEventId))
                {
                    var missedEvents = orderEventBuffer.GetEventsAfter(lastEventId);
                    foreach (var missedEvent in missedEvents)
                    {
                        yield return missedEvent;
                    }
                }

                await foreach (var order in channelReader.ReadAllAsync(cancellationToken))
                {
                    yield return orderEventBuffer.Add(order);
                }
            }
            return TypedResults.ServerSentEvents(StreamEvents(), "orders");
        });
    }
}
