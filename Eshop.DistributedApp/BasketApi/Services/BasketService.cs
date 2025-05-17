using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BasketApi.Services;

public class BasketService : IBasketService
{
    private readonly IDistributedCache _cache;
    private readonly CatalogApiClient _catalogApiClient;
    private readonly ILogger<BasketService> _logger;

    public BasketService(IDistributedCache cache, CatalogApiClient catalogApiClient, ILogger<BasketService> logger)
    {
        _cache = cache;
        _catalogApiClient = catalogApiClient;
        _logger = logger;
    }   

    public async Task<ShoppingCart?> GetBasket(string userName)
    {
       _logger.LogInformation($"Getting Basket ShoppingCart for user: {userName}");

       var basket = await _cache.GetStringAsync(userName);
       
        return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    /// <summary>
    /// Upsert Opeartion for Cache
    /// </summary>
    /// <param name="basket"></param>
    /// <returns></returns>
    public async Task UpdateBasket(ShoppingCart basket)
    {
        // Before update into Shopingchart, get the realtime price for the product and update the Shopping cart and then update the redis
        _logger.LogInformation("Calling CatalogApi for getting the item price");
        foreach (var item in basket.Items)
        {
            _logger.LogInformation("Getting PriceInfo for ProductId:{ProductId}", item.ProductId);
            var product = await _catalogApiClient.GetProductById(item.ProductId);
            item.Price = product.Price;
            item.ProductName = product.Name;
        }

        _logger.LogInformation("Updating ShoppingCart");
        await _cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
    }

    public async Task DeleteBasket(string userName)
    {
        _logger.LogInformation($"Deleting Basket entry for {userName}");
        await _cache.RemoveAsync(userName);
    }
}
