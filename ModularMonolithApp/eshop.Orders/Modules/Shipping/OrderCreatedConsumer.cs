using eshop.Orders.Modules.Orders.PublicApi;
using eshop.Orders.Modules.Shipping.Models;
using MassTransit;

namespace eshop.Orders.Modules.Shipping;

public class OrderCreatedConsumer(IOrderService orderService, ShippingDbContext shippingDbContext) 
        : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var orderShippingInfo = await orderService.GetOrderForShippingAsync(context.Message.OrderId);

        if(orderShippingInfo == null)
        {
            throw new InvalidOperationException($"Order {context.Message.OrderId} not found");
        }

        var shipment = new ShipmentRecord
        {
            OrderId = orderShippingInfo.OrderId,
            ShippingAddress = orderShippingInfo.ShippingAddress,
            TrackingNumber = $"TRACK-{Guid.NewGuid():N}",
            CreatedAt = DateTime.UtcNow,
            Status = ShipmentStatus.Pending
        };

        shippingDbContext.ShipmentRecords.Add(shipment);
        await shippingDbContext.SaveChangesAsync();
    }
}
