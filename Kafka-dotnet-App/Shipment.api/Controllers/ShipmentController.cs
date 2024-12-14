using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipment.api.Services;

namespace Shipment.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly ILogger<ShipmentController> _logger;
        private readonly IShippingService _service;

        public ShipmentController(ILogger<ShipmentController> logger,
                                IShippingService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> StartConsuming()
        {
            await _service.ConsumeOrders();
            return NoContent();
        }
    }
}
