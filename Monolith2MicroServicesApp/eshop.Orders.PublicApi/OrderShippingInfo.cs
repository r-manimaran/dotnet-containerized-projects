namespace eshop.Orders.PublicApi;

public record OrderShippingInfo (Guid OrderId, string ShippingAddress);
