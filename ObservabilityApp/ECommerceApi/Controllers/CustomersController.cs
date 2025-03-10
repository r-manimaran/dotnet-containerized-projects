using ECommerceApi.DTOs;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerRequest customerRequest)
        {
            var result = await _customerService.CreateCustomer(customerRequest);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var results = await _customerService.GetCustomers();
            return Ok(results);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await _customerService.GetCustomerById(id);
            return Ok(result);
        }
    }
}
