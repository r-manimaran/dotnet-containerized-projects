using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApi.Data;
using SalesApi.Dtos;
using SalesApi.Models;

namespace SalesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly SalesDbContext _dbContext;

        public SalesController(ILogger<SalesController> logger, SalesDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder(SalesRequest request)
        {

            var newRequest = new Sale
            {
                UserId = request.UserId,
                CreatedOn = DateTime.UtcNow,
                Products = request.Products
            };
             _dbContext.Sales.Add(newRequest);
            await _dbContext.SaveChangesAsync();
            return Ok(newRequest);
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrders()
        {
            var orders = await _dbContext.Sales.ToListAsync();

            return Ok(orders);
        }
    }
}
