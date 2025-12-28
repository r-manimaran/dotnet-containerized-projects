using OrdersApi.Models;
using System.Threading.Channels;

namespace OrdersApi.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("orders/realtime", async (ChannelReader<Order> channelReader, 
            CancellationToken cancellationToken) =>
        {
            //return Results.ServerSentEvents
            throw new NotImplementedException();
        });
    }
}
