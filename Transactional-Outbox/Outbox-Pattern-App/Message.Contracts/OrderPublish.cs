namespace Message.Contracts
{
    public class OrderPublish
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } =string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal TotalPrice {  get; set; }
    }
}
