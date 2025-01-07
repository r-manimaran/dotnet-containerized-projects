using SharedLib.Enums;
using SharedLib.Models;

namespace BasketApi.Models;

public class Basket : EntityBase
{
    public BasketStatus Status { get; set; }
    public int CustomerId { get; set; }
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public decimal TotalPrice { get; set; }
}
