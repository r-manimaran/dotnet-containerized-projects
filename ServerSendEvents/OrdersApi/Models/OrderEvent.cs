namespace OrdersApi.Models;

public record OrderEvent(string eventType, Order order, string? Reason = null);

public static class OrderEventTypes
{
    public const string Created = "Order-Created";
    public const string Updated = "Order-Updated";
    public const string Cancelled = "Order-Cancelled";
}