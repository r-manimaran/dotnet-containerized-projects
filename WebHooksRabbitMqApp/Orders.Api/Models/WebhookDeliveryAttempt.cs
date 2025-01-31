namespace Orders.Api.Models
{
    public class WebhookDeliveryAttempt
    {
        public Guid Id { get; set; }
        public Guid WebhookSubscriptionId { get; set; }
        public string Payload { get; set; }
        public int? ResponseStatusCode { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
