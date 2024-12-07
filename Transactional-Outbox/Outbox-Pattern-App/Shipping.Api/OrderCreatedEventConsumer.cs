using System.Text.Json;
using MassTransit;
using Message.Contracts;
using Shipping.Api.Models;

namespace Shipping.Api;

public class OrderCreatedEventConsumer : IConsumer<OrderPublish>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;

    public OrderCreatedEventConsumer(AppDbContext dbContext,
                                    ILogger<OrderCreatedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPublish> context)
    {
        var orderId = context.Message.OrderId;
        var message = context.Message;
        
        _logger.LogInformation("Processing Order {orderId}", orderId);

        // Create JSON options for pretty printing
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Serialize the full message to JSON
            var jsonMessage = JsonSerializer.Serialize(message, jsonOptions);
            
            // Log the formatted JSON message
            _logger.LogInformation(
                "Received message: {JsonMessage}", 
                jsonMessage
            );

        //Insert to Shipment table
        var shipment = new Shipment
        {
            OrderId = orderId,
            ShippingStatus = ShipmentStatus.Pending.ToString(),
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        await _dbContext.Shipments.AddAsync(shipment);
        _dbContext.SaveChanges();
        _logger.LogInformation("Shipment created for order:{OrderId}",orderId);        
    }
}

public enum ShipmentStatus
{
    Pending,
    Shipped
}

