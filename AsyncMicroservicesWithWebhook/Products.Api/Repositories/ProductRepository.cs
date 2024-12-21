using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.DTOs;
using Shared.Contracts.Models;

namespace Products.Api.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly ProductDbContext _dbContext;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductRepository(ProductDbContext dbContext, 
                                 ILogger<ProductRepository> logger,
                                 IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
           _publishEndpoint = publishEndpoint;
        }
        public async Task<ServiceResponse> AddProductAsync(Product product)
        {
           _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Publish the new Product to RabbitMQ");
            await _publishEndpoint.Publish(product);
            _logger.LogInformation("Published to MessageQ successfully");
            return new ServiceResponse(true, "Product added Successfully");

        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _dbContext.Products.ToListAsync();
            return products;
        }
    }
}
