namespace Orders.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
