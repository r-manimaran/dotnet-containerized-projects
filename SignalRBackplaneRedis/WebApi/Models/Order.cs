namespace WebApi.Models;

public enum OrderStatus
{
    Created = 1,
    Pending = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Approved = 7
}
public class Order
{
    public Guid Id { get; set; }
    public string? UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public OrderStatus Status { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public List<StatusTransition> StatusTransitions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // calculate total price based on order items
    public decimal TotalAmount => OrderItems.Sum(item => item.UnitPrice * item.Quantity);
}

public class StatusTransition
{
    public OrderStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? InstanceId { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}