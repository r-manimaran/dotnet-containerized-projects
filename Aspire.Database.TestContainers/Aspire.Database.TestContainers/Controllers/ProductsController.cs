using Aspire.Database.TestContainers.Models;
using Aspire.Database.TestContainers.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aspire.Database.TestContainers.Controllers
{
    [Route("api/[controller]")]    
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await productService.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await productService.GetProductById(id);
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            var response = await productService.CreateProduct(product);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(Product product)
        {
            var response = await productService.DeleteProduct(product);
            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var response = await productService.UpdateProduct(product);
            return Ok(response);
        }

    }
}
