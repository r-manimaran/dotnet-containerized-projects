using SharedLib.Enums;
using SharedLib.Models;

namespace PaymentApi.Models;

public class Payment :EntityBase
{
    public string OrderId { get; set; }    
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string CardHolderName { get; set; }
    public string CVV { get; set; }
    public string ZipCode { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }

    public int CustomerId { get; set; }
}
