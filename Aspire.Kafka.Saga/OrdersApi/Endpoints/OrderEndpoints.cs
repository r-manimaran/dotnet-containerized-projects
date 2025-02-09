using Confluent.Kafka;
using eCommerce.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrdersApi.Data;

using SharedLib.Models;
using System.Text.Json.Serialization;

namespace OrdersApi.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
                       .WithTags("Orders")
                       .WithOpenApi();
        group.MapGet("/", async (OrderDbContext dbContext) =>
        {
            var orders = await dbContext.Orders.ToListAsync();
            return orders;
        });
        // Order Post
        group.MapPost("/", async (OrderDbContext dbContext, IKafkaProducer kafkaProducer, Order order) =>
        {
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var orderMessage = new OrderMessage
            {
                OrderId = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
            };

            await kafkaProducer.ProduceAsync("order-created", orderMessage);

            return Results.Ok(order);
            //return Results.Created($"/orders/{order.Id}", order);
        });
        
    }
}
