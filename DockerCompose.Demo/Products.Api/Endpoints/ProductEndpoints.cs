using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using Products.Api.Contracts;
using Products.Api.Database;
using Products.Api.Entities;

namespace Products.Api.Endpoints
{
    public static class ProductEndpoints
    {
        public static async void MapProductsEndpoints(this IEndpointRouteBuilder app) 
        {
            // Create Products
            app.MapPost("api/products", async (
                      CreateProductRequest request,
                      ApplicationDbContext dbContext,
                      CancellationToken ct) =>
            {
                var product = new Product
                {
                    Name = request.Name,
                    Price = request.Price,
                };

                dbContext.Add(product);
                await dbContext.SaveChangesAsync(ct);
                return Results.Ok(product);
            });

            // Get products
            app.MapGet("api/products", async (
                            ApplicationDbContext dbContext,
                            CancellationToken ct,
                            int page = 1,
                            int pageSize = 10) =>
            {
                var products = await dbContext.Products
                                .AsNoTracking()
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
                return Results.Ok(products);
            });

            // Get Product by Id
            app.MapGet("api/products/{id}", async (
                                    int id,
                                    ApplicationDbContext dbContext,
                                    IDistributedCache cache,
                                    CancellationToken ct) =>
            {
                            var cacheKey = $"products-{id}";

            // Try to get from cache first
            var cachedProduct = await cache.GetAsync(cacheKey, ct);
            Product? product = null;

            if (cachedProduct == null)
            {
                // If not in cache, get from database
                product = await dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id, ct);

                if (product != null)
                {
                    // Serialize the product to store in cache
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Or whatever duration you prefer
                    };

                    var serializedProduct = JsonSerializer.Serialize(product);
                    var productBytes = Encoding.UTF8.GetBytes(serializedProduct);

                    // Store in cache
                    await cache.SetAsync(cacheKey, productBytes, options, ct);
                }
            }
            else
            {
                // Deserialize from cache
                var serializedProduct = Encoding.UTF8.GetString(cachedProduct);
                product = JsonSerializer.Deserialize<Product>(serializedProduct);
            }

            return product is null ? Results.NotFound() : Results.Ok(product);

            });

            // update Product
            app.MapPut("api/products/{id}", async (
                                int id,
                                UpdateProductRequest request,
                                ApplicationDbContext dbContext,
                                IDistributedCache cache,
                                CancellationToken ct) => 
            {
                var product = await dbContext.Products                                
                                .FirstOrDefaultAsync(p => p.Id == id,ct);

                if(product is null)
                {
                    return Results.NotFound();
                }

                product.Name = request.Name;
                product.Price = request.Price;

                await dbContext.SaveChangesAsync(ct);
                await cache.RemoveAsync($"products-{id}");
                return Results.NoContent();

            });

            // Delete Product
            app.MapDelete("api/products/{id}", async (int id,
                                                ApplicationDbContext dbContext,
                                                IDistributedCache cache,
                                                CancellationToken ct) =>
            {
                var product = await dbContext.Products
                                .FirstOrDefaultAsync(p => p.Id == id, ct);
                if(product is null)
                {
                    Results.NotFound();
                }

                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync(ct);

                await cache.RemoveAsync($"products-{id}");

                return Results.NoContent();
            });
        }
    }
}
