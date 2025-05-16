namespace BasketApi.Services;

public interface IBasketService
{
    Task<ShoppingCart> GetBasket(string userName);
    Task UpdateBasket(ShoppingCart basket);

    Task DeleteBasket(string userName);
}
