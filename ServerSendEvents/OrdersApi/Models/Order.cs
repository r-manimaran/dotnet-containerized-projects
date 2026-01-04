namespace OrdersApi.Models;

public record Order (
    string OrderId,
    string CustomerName,
    decimal Amount,
    DateTime OrderDate,
    OrderStatus Status = OrderStatus.New
);

public enum OrderStatus
{
    New,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
