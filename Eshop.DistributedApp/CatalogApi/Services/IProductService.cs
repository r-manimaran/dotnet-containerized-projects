namespace CatalogApi.Services;

public interface IProductService
{
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task<Product?> GetById(int id);
    Task<IEnumerable<Product>> GetAll();
    Task DeleteProductAsync(int id);
    Task<IEnumerable<Product>> SearchProductsAsync(string query);
}
