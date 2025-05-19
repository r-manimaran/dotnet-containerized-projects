using Eshop.DistributedApp.ServiceDefaults.Messaging.Events;
using MassTransit;

namespace BasketApi.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler(
    IBasketService basketService, 
    ILogger<ProductPriceChangedIntegrationEventHandler> logger) 
    : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        logger.LogInformation("Consuming the Product Price Changed Event");

        await  basketService.UpdateBasketItemProductPrices(context.Message.ProductId, context.Message.Price);
    }
}
