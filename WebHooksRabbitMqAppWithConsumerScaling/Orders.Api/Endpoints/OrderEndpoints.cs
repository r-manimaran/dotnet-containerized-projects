using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Models;
using Orders.Api.Repositories;
using Orders.Api.Services;

namespace Orders.Api.Endpoints
{
    public static class OrderEndpoints
    {
        // In-memory Implementation endpoint
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/Orders", (InMemoryOrderRepository orderRepository) =>
            {
                var orders = orderRepository.GetAll();
                return Results.Ok(orders);
            }).WithTags("Orders");

            app.MapPost("/Order", async (CreateOrderRequest request, 
                                          InMemoryOrderRepository orderRepository,
                                          WebhookDispatcher webhookDispatcher) =>
            {
                var newOrder = new Order(
                    Id: Guid.NewGuid(),
                    CustomerName : request.CustomerName,
                    Amount: request.Amount,
                    CreatedAt: DateTime.UtcNow
                );

                orderRepository.AddOrder(newOrder);
                await webhookDispatcher.DispatchAsync("order.created", newOrder);
                return Results.Ok(newOrder);
            }).WithTags("Orders"); ;
        }

        // Postgres Database data persistance implementation
        public static void MapPersistentOrderEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/Orders", async (WebhookDbContext dbContext) =>
            {
                var orders = await dbContext.Orders.ToListAsync();
                return Results.Ok(orders);
            }).WithTags("Orders");


            app.MapPost("/Order", async (CreateOrderRequest request,
                                          WebhookDbContext dbContext,
                                          WebhookDispatcherUsingDB webhookDispatcher) =>
            {
                var newOrder = new Order(
                    Id: Guid.NewGuid(),
                    CustomerName: request.CustomerName,
                    Amount: request.Amount,
                    CreatedAt: DateTime.UtcNow
                );

                dbContext.Orders.Add(newOrder);
                
                await webhookDispatcher.DispatchAsync("order.created", newOrder);
                return Results.Ok(newOrder);
            }).WithTags("Orders");
        }
    }
}
