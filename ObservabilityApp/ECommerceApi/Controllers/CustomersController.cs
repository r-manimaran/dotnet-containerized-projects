using ECommerceApi.DTOs;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        Meter customMeter = new Meter("CustomerApi", "1.0.0");
        Counter<int> customersCreatedCount;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
            customersCreatedCount = customMeter.CreateCounter<int>(
          name: "Total_Customers",
          unit: "Customers",
          description: " The number of customers created");
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerRequest customerRequest)
        {
            var result = await _customerService.CreateCustomer(customerRequest);
            customersCreatedCount.Add(1);
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
