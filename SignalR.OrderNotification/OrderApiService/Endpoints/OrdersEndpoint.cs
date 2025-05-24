using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using OrderApiService.Hubs;
using OrderApiService.Models;
using OrderApiService.Utilities;
using System.Security.Claims;
using System.Text.Json;

namespace OrderApiService.Endpoints;

public static class OrdersEndpoint
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/{id}", async (Guid id, IDistributedCache cache) =>
        {
            var cacheKey = $"Order_{id}";
            var orderJson = await cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(orderJson))
            {
                return Results.NotFound(new { error = $"Order with ID {id} not found." });
            }
            var order = JsonSerializer.Deserialize<Order>(orderJson);
            return Results.Ok(order);
        })
        .WithName("GetOrderById")
        .Produces<List<Order>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound); 
    

        group.MapPost("/", async (IDistributedCache cache, ClaimsPrincipal claimsPrincipal) =>
        {
            var createdOrder = DummyDataGenerator.GenerateDummyOrder();
            createdOrder.UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var cacheKey = $"Order_{createdOrder.Id}";
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(createdOrder), cacheOptions);

          
            return Results.Created($"/orders/{createdOrder.Id}", createdOrder);
        })
        .WithName("CreateOrder")
        .Produces<Order>(StatusCodes.Status201Created)
        .RequireAuthorization();


        group.MapPut("/{id}", async (Guid id,
            OrderStatus status,
            IDistributedCache cache,
            IHubContext<OrderNotificationHub, IOrderNotificationClient> hubContext,
            ClaimsPrincipal claimsPrincipal) =>
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return Results.BadRequest(new { error = $"Invalid order status:{status}" });
            }
            var cacheKey = $"Order_{id}";
            var orderJson = await cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(orderJson))
            {
                return Results.NotFound(new { error = $"Order with ID {id} not found." });
            }
            var order = JsonSerializer.Deserialize<Order>(orderJson);
            if (order is null)
            {
                return Results.NotFound(new { error = $"Order with ID {id} not found." });
            }
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(order), options);

            // Notify all clients about the order status update
            //await hubContext.Clients.All.SendAsync("OrderStatusUpdated", order);

            //await hubContext.Clients.All.OrderStatusUpdated(order);

            // Notify the specific user about the order status update
            await hubContext.Clients.User(order.UserId!).OrderStatusUpdated(order);

            return Results.Ok(order);
        })
        .WithName("UpdateOrder")
        .Produces<Order>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        


       
    }
}
