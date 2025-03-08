namespace eshop.Orders.Modules.Orders.PublicApi;

public class OrderShippingInfo
{
    public Guid OrderId { get; set; }
    public string ShippingAddress { get; set; }
}
