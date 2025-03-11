using eshop.Orders.PublicApi;

namespace eshop.Shipping.Api.Services;

public class OrderService(IHttpClientFactory httpClientFactory, ILogger<OrderService> logger) : IOrderShippingService
{
    public async Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId)
    {
        
        try
        {
            using var client = httpClientFactory.CreateClient("orders");
            logger.LogInformation("Attempting to fetch order {OrderId} from {BaseAddress}", 
             orderId, client.BaseAddress);
            var order = await client.GetFromJsonAsync<OrderShippingInfo>($"orders/{orderId}/shipping-info");

            return order;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.ToString());
            logger.LogError(ex, "Error while getting order from orders service for order {OrderId}. Error:{Error}",
                orderId, ex.Message);
            throw;
        }
    }
}
