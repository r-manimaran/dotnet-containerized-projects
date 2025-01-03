using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OrderProcessingService.Saga;
using OrderProcessingService.Services;
using Rebus.Bus;

namespace OrderProcessingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBus _bus;
        public OrdersController(IOrderRepository orderRepository, IBus bus)
        {
            _orderRepository = orderRepository;
            _bus = bus;
        }
        
        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrderById(string orderId)
        {
            if(string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Order Id is required");
            }
            if(!Guid.TryParse(orderId, out var orderGuid))
            {
                return BadRequest("Invalid Order Id format");
            }

             var order = _orderRepository.GetOrderById(orderGuid);
             if(order != null)
                return Ok(order);
            
            return NotFound($"Order with Id {orderId} not exists!");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            await _bus.Send(new PlaceOrderCommand
            {
                OrderId = orderId,
            });
            return CreatedAtAction(nameof(GetOrderById), new { orderId },null);
        }

        [HttpPost("{orderId}/payment")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePayment(string orderId)
        {
            if(Guid.TryParse(orderId, out var orderGuid))
            {
                var order = _orderRepository.GetOrderById(orderGuid);
                await _bus.Send(new ProcessPaymentCommand
                {
                    OrderId = order.OrderId,
                });

                return Accepted();
            }
            return BadRequest("Invalid Order Id format");
        }
    }
}
