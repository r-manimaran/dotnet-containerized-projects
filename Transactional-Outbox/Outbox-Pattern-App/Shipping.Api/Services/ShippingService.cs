using Microsoft.EntityFrameworkCore;
using Shipping.Api.Models;

namespace Shipping.Api.Services;

public class ShippingService : IShippingService
{
    private readonly ILogger<ShippingService> _logger;
    private readonly AppDbContext _dbContext;

    public ShippingService(ILogger<ShippingService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<Shipment> GetByOrderIdAsync(Guid OrderId)
    {
        var shipmentItem = await _dbContext.Shipments.FirstOrDefaultAsync(x => x.OrderId == OrderId);
        if (shipmentItem == null)
        {
            _logger.LogInformation("Unable to find the Order with {OrderId}", OrderId);
            throw new Exception("Order not found.");
        }

        return shipmentItem;
    }
}
