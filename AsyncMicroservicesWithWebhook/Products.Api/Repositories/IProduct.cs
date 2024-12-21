using Shared.Contracts.DTOs;
using Shared.Contracts.Models;

namespace Products.Api.Repositories
{
    public interface IProduct
    {
        Task<ServiceResponse> AddProductAsync(Product product);
        Task<List<Product>> GetProductsAsync();
    }
}
