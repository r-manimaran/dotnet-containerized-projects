namespace eshop.Orders.Modules.Shipping.Models;

public class ShipmentRecord
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ShippingAddress { get; set; }
    public string TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Status { get; set; }
}

public static class ShipmentStatus
{
    public const int Pending = 0;
    public const int Shipped = 1;
}