namespace CatalogApi.Services;

public interface IProductService
{
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task<Product> GetById(int id);
    Task<List<Product>> GetAll();
    Task DeleteProductAsync(int id);
}
