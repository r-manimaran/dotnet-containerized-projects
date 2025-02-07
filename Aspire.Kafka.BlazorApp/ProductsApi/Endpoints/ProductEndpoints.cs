using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;

namespace ProductsApi.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                       .WithTags("Product")
                       .WithOpenApi();

        group.MapGet("/all", async(ProductDbContext dbContext)=>
        {
            var products = await dbContext.Products.ToListAsync();
            return products;
        }).WithName("GetAllProducts");

        // Get Product by Id
        group.MapGet("/{id}", async (ProductDbContext dbContext, int id) =>
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(product);
        }).WithName("GetProductById");
    }    
}
