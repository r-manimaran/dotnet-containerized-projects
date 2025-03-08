using eshop.Orders.Modules.Orders.Models;
using eshop.Orders.Modules.Orders.PublicApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eshop.Orders.Modules.Orders
{
    public static class Endpoints
    {
        public static void MapOrdersEndpoints(this IEndpointRouteBuilder route)
        {
            var app = route.MapGroup("api/orders").WithOpenApi().WithTags("Orders");
            app.MapPost("", async (CreateOrderDto order, IOrderService orderService) =>
            {
                var result = await orderService.CreateOrder(order);

                return Results.Created($"orders/{result.Id}", result);
            });

            app.MapGet("/{id:guid}", async (Guid id, IOrderService orderService) =>
            {
                var order = await orderService.GetOrder(id);
                if(order==null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(order);
            });
        }
    }
}
