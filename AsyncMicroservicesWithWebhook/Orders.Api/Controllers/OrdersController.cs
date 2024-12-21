using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Repositories;
using Shared.Contracts.DTOs;
using Shared.Contracts.Models;

namespace Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrder _orderService;

        public OrdersController(ILogger<OrdersController> logger, IOrder order)
        {
            _logger = logger;
            _orderService = order;
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> CreateOrder(OrderRequest newOrder)
        {

            var response = await _orderService.AddOrderAsync(newOrder);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
