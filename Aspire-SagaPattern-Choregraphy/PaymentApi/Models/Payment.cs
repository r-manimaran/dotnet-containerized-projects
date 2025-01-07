using SharedLib.Enums;
using SharedLib.Models;

namespace PaymentApi.Models;

public class Payment :EntityBase
{
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string NameOnCard { get; set; }
    public string CVV { get; set; }
    public string ZipCode { get; set; }
    public double Balance { get; set; }
    public PaymentStatus Status { get; set; }

    public int CustomerId { get; set; }
}
