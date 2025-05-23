using CatalogApi.Services;

namespace CatalogApi.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithOpenApi().WithTags("Products");

        group.MapGet("/", async (IProductService productServie) =>
        {
            var products = await productServie.GetAll();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("/{Id}", async (int Id, IProductService productService) =>
        {
            var product = await productService.GetById(Id);
            if(product == null) return Results.NotFound();

            return Results.Ok(product);

        }).WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (Product product, IProductService productService) =>
        {
            await productService.CreateProductAsync(product);
            return Results.Created($"/api/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapPut("/", async (Product product, IProductService productService) =>
        {

            await productService.UpdateProductAsync(product);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/{Id:int}", async (int Id, IProductService productService) =>
        {
            await productService.DeleteProductAsync(Id);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Support Endpoint
        group.MapGet("/support/{query}", async (string query, IProductAIService productAIService) =>
        {
            var result = await productAIService.SupportAsync(query);

            return Results.Ok(result);
        })
        .WithName("Support")
        .Produces(StatusCodes.Status200OK);

        // Keyword Search Endpoint
        group.MapGet("/search/{query}", async (string query, IProductService service) =>
        {
            var products = await service.SearchProductsAsync(query);

            return Results.Ok(products);

        }).WithName("SearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);


        // Semantic Search Endpoint
        group.MapGet("/aisearch/{query}", async (string query, IProductAIService service) =>
        {
            var products = await service.SearchProductsAsync(query);

            return Results.Ok(products);

        }).WithName("AISearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);
    }
}
