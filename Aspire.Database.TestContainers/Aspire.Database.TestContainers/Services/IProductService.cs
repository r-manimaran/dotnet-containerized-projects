using Aspire.Database.TestContainers.DTOs;
using Aspire.Database.TestContainers.Models;

namespace Aspire.Database.TestContainers.Services;

public interface IProductService 
{
    Task<ServiceResponse<Product>> CreateProduct(Product product);
    Task<ServiceResponse<Product>> UpdateProduct(Product product);
    Task<ServiceResponse<Product>> DeleteProduct(Product product);
    Task<ServiceResponse<Product>> GetProductById(int productId);
    Task<ServiceResponse<List<Product>>> GetProducts();
}
