using CustomerApi.DTOs;
using CustomerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            var result = await customerService.CreateCustomer(request);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await customerService.GetAllCustomers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await customerService.GetCustomerById(id);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCustomer(UpdateCustomerRequest request)
        {
            var result = await customerService.UpdateCustomer(request);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await customerService.DeleteCustomer(id);
            return Ok();
        }
    }
}
