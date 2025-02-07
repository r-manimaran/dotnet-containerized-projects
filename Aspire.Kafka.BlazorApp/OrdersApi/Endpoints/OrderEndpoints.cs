using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrdersApi.Data;
using OrdersApi.Kafka;
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
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            await kafkaProducer.ProduceAsync("order-topic", new Message<string, string>
            {
                Key = order.Id.ToString(),
                Value = JsonConvert.SerializeObject(order)
            });

            return Results.Ok(order);
            //return Results.Created($"/orders/{order.Id}", order);


        });
        
    }
}
