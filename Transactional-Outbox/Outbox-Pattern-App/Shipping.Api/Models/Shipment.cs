using System.ComponentModel.DataAnnotations;

namespace Shipping.Api.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }

        [MaxLength(50)]
        public string ShippingStatus { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
