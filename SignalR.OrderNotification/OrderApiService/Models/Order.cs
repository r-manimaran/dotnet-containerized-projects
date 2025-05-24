namespace OrderApiService.Models;

public class Order
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // calculate total price based on order items
    public decimal TotalAmount => OrderItems.Sum(item => item.UnitPrice * item.Quantity);
}
