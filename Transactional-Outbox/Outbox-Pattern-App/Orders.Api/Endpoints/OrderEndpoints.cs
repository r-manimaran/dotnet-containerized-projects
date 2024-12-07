using Microsoft.AspNetCore.Mvc;
using Orders.Api.DTOs;
using Orders.Api.Services;

namespace Orders.Api.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/Order", async ([FromBody] OrderRequest newOrder,IOrderService orderService) =>
            {
                var response = await orderService.CreateNewOrderAsync(newOrder);

                return Results.Ok(response);

            }).WithName("CreateOrder");



            app.MapGet("/Order/{id}", async (Guid id, IOrderService orderService) =>
            {
                var response = await orderService.GetOrderAsync(id);
                return Results.Ok(response);
            }).WithName("GetOrderById");
        }
    }
}
