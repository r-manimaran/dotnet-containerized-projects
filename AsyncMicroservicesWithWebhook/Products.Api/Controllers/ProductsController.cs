using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Repositories;
using Shared.Contracts.DTOs;
using Shared.Contracts.Models;

namespace Products.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProduct _product;

        public ProductsController(ILogger<ProductsController> logger, IProduct product)
        {
            _logger = logger;
            _product = product;
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddProduct([FromBody]Product product)
        {
            var response = await _product.AddProductAsync(product);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts() => 
            await _product.GetProductsAsync();
    }
}
