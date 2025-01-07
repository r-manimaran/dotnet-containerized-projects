using SharedLib.Models;

namespace InventoryApi.Models;

public class Product : EntityBase
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
