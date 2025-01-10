using SharedLib.Enums;
using SharedLib.Models;
using System.Net.NetworkInformation;

namespace BasketApi.Models;

public class Basket : EntityBase
{
    public BasketStatus Status { get; set; }
    public int CustomerId { get; set; }
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public decimal TotalPrice { get; set; }

    public static Basket Create (int customerId, BasketStatus status)
    {
        return new Basket
        {
            CustomerId = customerId,
            Status = status,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "system",
            ModifiedBy = "system",
            ModifiedOn = DateTime.UtcNow,
        };
    }

    public void AddBasketItem(int productId, int basketId, decimal price, int count, BasketStatus basketStatus)
    {
        BasketItems.Add(new BasketItem(productId, basketId, price, count, basketStatus));
    }
}
