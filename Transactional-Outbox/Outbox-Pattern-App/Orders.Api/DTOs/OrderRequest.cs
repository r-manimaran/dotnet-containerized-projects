namespace Orders.Api.DTOs
{
    public class OrderRequest
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
