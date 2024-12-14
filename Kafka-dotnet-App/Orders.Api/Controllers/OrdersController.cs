using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.DTO;
using Orders.Api.Services;

namespace Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;

        public OrdersController(ILogger<OrdersController> logger, 
                                IOrderService orderService) 
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]OrderRequest orderRequest)
        {
            var response = await _orderService.CreateOrder(orderRequest);
            return Ok(response);
        }
    }
}
