namespace Message.Contract
{
    public class OrderReceivedMessage
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public List<OrderProduct> Products { get; set; } = new List<OrderProduct>();
        public decimal TotalPrice { get; set; }
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }

    public class OrderProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
