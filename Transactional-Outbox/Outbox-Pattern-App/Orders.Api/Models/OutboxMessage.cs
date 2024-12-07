namespace Orders.Api.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; init; }
        /// <summary>
        /// Type of the message 
        /// </summary>
        public required string Type { get; set; }
        /// <summary>
        /// For Serialized Json Content for message get published
        /// </summary>
        public required string Content { get; set; }
        public DateTime OccuredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        /// <summary>
        /// If processing failed, this will contains some errors
        /// </summary>
        public string? Error { get; set; }

    }
}
