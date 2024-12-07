using Shipping.Api.Models;

namespace Shipping.Api.Services
{
    public interface IShippingService
    {
        Task<Shipment> GetByOrderIdAsync(Guid OrderId);
    }
}
