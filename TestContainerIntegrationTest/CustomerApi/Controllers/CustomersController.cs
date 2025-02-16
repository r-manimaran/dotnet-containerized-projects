using System.Diagnostics;
using System.Diagnostics.Metrics;
using CustomerApi.DTOs;
using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private static readonly Meter MyMeter= new("MyApi.CustomersService","1.0.0");
        private static readonly Counter<int> CustomerCreationCounter = MyMeter.CreateCounter<int>("Customer_creations_total",
        description:"Number of customers created");
        private static readonly Histogram<double> CustomerCreationDuration = MyMeter.CreateHistogram<double>(
    "customer_creation_duration_seconds",
    description: "Time taken to create customers",
    unit: "s");
        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
        {
               var timer = new Stopwatch();
               timer.Start();
            var result = await customerService.CreateCustomer(request);

            CustomerCreationCounter.Add(1, new KeyValuePair<string, object?>[]
            {
                new("customer_type",request.GetType()),
                new("source","customerApi")
            });
            timer.Stop();
            CustomerCreationDuration.Record(timer.Elapsed.TotalSeconds, new KeyValuePair<string, object?>[]
            {
                new("customer_type", request.GetType()),             
            });
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
