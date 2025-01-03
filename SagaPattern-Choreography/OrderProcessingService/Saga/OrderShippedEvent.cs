namespace OrderProcessingService.Saga;

public class OrderShippedEvent
{
    public Guid OrderId { get; set; }
}
