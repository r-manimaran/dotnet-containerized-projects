using InventoryApi.DTOs;

namespace InventoryApi.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Products");

        group.MapGet("/{id}", (int id) =>
        {
            throw new NotImplementedException();
        });

        group.MapPost("", (CreateProductRequest request) =>
        {
            throw new NotImplementedException();
        });

        group.MapPut("/{id}", (int id, CreateProductRequest request) =>
        {
            throw new NotImplementedException();
        });

        group.MapDelete("/{id}",(int id) =>
        {
            throw new NotImplementedException(); 
        });
    }
}
