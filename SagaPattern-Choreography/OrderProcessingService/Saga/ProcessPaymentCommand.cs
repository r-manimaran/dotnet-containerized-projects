namespace OrderProcessingService.Saga;

public class ProcessPaymentCommand
{
    public Guid OrderId { get; set; }
}
