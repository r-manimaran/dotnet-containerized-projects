namespace OrdersApi.Models;

public record Order (
    string OrderId,
    string CustomerName,
    decimal Amount,
    DateTime OrderDate
);
