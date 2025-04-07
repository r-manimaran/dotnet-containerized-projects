using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;

namespace ProductsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductDbContext _dbContext;

    public ProductsController(ILogger<ProductsController> logger, ProductDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductRequest request)
    {
        _logger.LogInformation("Getting the request {Request} to create product.", request);
        var newProduct = new Product
        {
            Name = request.Name,
            Quantity = request.Quantity,
            Price = request.Price,
            CreatedOn = DateTime.UtcNow
        };

        _dbContext.Products.Add(newProduct);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Response generated from CreateProduct {CreateProductReponse}", newProduct);
        return Ok(newProduct);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {

        var products = await _dbContext.Products.ToListAsync();
        return Ok(products);
    }
}
