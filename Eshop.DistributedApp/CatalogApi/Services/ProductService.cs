namespace CatalogApi.Services;

public class ProductService(ProductDbContext dbContext, ILogger<ProductService> logger) : IProductService
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

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await _dbContext.Products.SingleOrDefaultAsync(i=>i.Id==product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;

            _dbContext.Products.Update(existingProduct);
            await _dbContext.SaveChangesAsync();
        }
    }
}
