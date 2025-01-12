namespace BasketApi.Dtos;

public class PaymentDto
{
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string CardHolderName { get; set; }
    public string CVV { get; set; }
    public string ZipCode { get; set; }
    public decimal Amount { get; set; }
}
