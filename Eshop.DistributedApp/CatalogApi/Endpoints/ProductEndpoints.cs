namespace CatalogApi.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductsEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithOpenApi().WithTags("Products");

        group.MapGet("/")
    }
}
