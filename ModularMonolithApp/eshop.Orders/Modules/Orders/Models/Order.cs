namespace eshop.Orders.Modules.Orders.Models;

public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
}

public class CreateOrderDto
{
    public string CustomerName { get; set; }
    public string ShippingAddress { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
