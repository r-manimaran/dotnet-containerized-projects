using MassTransit;

namespace CatalogApi.Services;

public class ProductService(ProductDbContext dbContext, ILogger<ProductService> logger, IBus bus) : IProductService
{
    private readonly ProductDbContext _dbContext = dbContext;
    private readonly ILogger<ProductService> _logger = logger;

    public async Task CreateProductAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();        
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _dbContext.Products.SingleOrDefaultAsync(i => i.Id == id);
        if (product != null)
        {
            _dbContext.Products.Remove(product);            
        }
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var products = await _dbContext.Products.ToListAsync();
        return products;
    }

    public async Task<Product?> GetById(int id)
    {
        var product = await _dbContext.Products.SingleOrDefaultAsync(i=>i.Id == id);

        return product;
    }
    //keyword search
    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        return await _dbContext.Products
            .Where(i => i.Name.Contains(query) || i.Description.Contains(query))
            .ToListAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await _dbContext.Products.SingleOrDefaultAsync(i=>i.Id==product.Id);
        
        
        if (existingProduct != null)
        {
            // If the existing Product price is different from updated Product
            // raise ProductPriceChanged event
            if (existingProduct.Price != product.Price)
            {
                var integrationEvent = new ProductPriceChangedIntegrationEvent
                {
                    ProductId = existingProduct.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                };

                await bus.Publish(integrationEvent);
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;

            _dbContext.Products.Update(existingProduct);
            await _dbContext.SaveChangesAsync();
        }
    }
}
