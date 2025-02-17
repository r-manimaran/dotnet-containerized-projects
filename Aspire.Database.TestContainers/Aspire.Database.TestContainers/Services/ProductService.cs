using Aspire.Database.TestContainers.Data;
using Aspire.Database.TestContainers.DTOs;
using Aspire.Database.TestContainers.Models;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Database.TestContainers.Services;

public class ProductService(AppDbContext dbContext, ILogger<ProductService> logger) : IProductService
{
    public async Task<ServiceResponse<Product>> CreateProduct(Product product)
    {
        var existingProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Name == product.Name);
        if (existingProduct != null) 
        {
            logger.LogError($"Product with name {product.Name} already exists.");
            return new ServiceResponse<Product>
            {
                Data = product,
                Success = false,
                Message = $"Product with name {product.Name} is already exists"
            };
        }
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"Product with name {product.Name} added successfully.");
        return new ServiceResponse<Product>
        {
            Data = product,
            Success = true,
            Message = "Product Created Successfully"
        };
    }

    public async Task<ServiceResponse<Product>> DeleteProduct(Product product)
    {
        var existingProduct = await dbContext.Products.FirstOrDefaultAsync(p=>p.Id == product.Id);
        if(existingProduct ==null)
        {
            logger.LogInformation($"Product with Id {product.Id} does not exists");
            return new ServiceResponse<Product>
            {
                Data = product,
                Success = false,
                Message = $"Product with Id {product.Id} does not exists"
            };
        }
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"{product.Name} deleted");
        return new ServiceResponse<Product>
        {
            Success = true,
            Message = $"Product with Id {product.Id} Deleted successfully"
        };
    }

    public async Task<ServiceResponse<Product>> GetProductById(int productId)
    {
        var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (existingProduct == null)
        {
            logger.LogError($"Product with Id {productId} does not exists");
            return new ServiceResponse<Product>
            {
                Data = null,
                Success = false,
                Message = $"Product with Id `{productId}` does not exists!"
            };
        }
        logger.LogInformation($"Got the Product with Id {productId} ");
        return new ServiceResponse<Product>
        {
            Data = existingProduct,
            Success = true,
        };
    }

    public async Task<ServiceResponse<List<Product>>> GetProducts()
    {
        var products = await dbContext.Products.ToListAsync();
        return new ServiceResponse<List<Product>>
        {
            Data = products,
            Success = true
        };
    }

    public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
    {
        var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
        if(existingProduct == null)
        {
            logger.LogInformation($"Product with Id {product.Id} does not exists");
            return new ServiceResponse<Product>
            {
                Success = false,
                Message = $"Product with Id {product.Id} does not exists"
            };
        }
        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Quantity = product.Quantity;
        dbContext.Products.Update(existingProduct);
        await dbContext.SaveChangesAsync();
        logger.LogInformation($"Updated the Product with Id {product.Id} successfully");
        return new ServiceResponse<Product>
        {
            Data = existingProduct,
            Success = true,
        };
    }
}
