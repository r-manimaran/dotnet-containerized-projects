using FluentValidation;
using InventoryApi.DTOs;
using InventoryApi.Exceptions;
using InventoryApi.Models;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Repository;

namespace InventoryApi.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products")
                                                        .WithOpenApi();
                                                        //.RequireAuthorization();


        group.MapGet("/", async ([FromServices] IRepository<Product> repository) =>
        {
            var products = await repository.GetAllAsync();
            return Results.Ok(products);
        })
         .WithName("GetAllProducts");


        group.MapGet("/{id}", async (int id, [FromServices] IRepository<Product> repository) =>
        {
            var product = await repository.GetByIdAsync(id);
            if (product == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(product);
        })
        .WithName("GetProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);


        group.MapGet("/filter/{price}", async (decimal price,
                            [FromServices] IRepository<Product> repostiory) =>
        {
            var products = await repostiory.GetAllFilter(p => p.Price <= price);
            return Results.Ok(products);
        })
        .WithName("GetProductsByPrice");
        

        group.MapPost("", async ([FromBody] CreateProductRequest request,
                                        [FromServices] IRepository<Product> repository,
                                        [FromServices] IValidator<CreateProductRequest> validator,
                                        [FromServices] ILogger<Product> logger,
                                        CancellationToken cancellationToken) =>
         {
             logger.LogTrace("Received Request to create a new Product");
             var validationResult = await validator.ValidateAsync(request,cancellationToken);
             if(!validationResult.IsValid)
             {
                 return Results.ValidationProblem(validationResult.ToDictionary());
             }

             var userName = "system"; // when using authentication change this to get from context.User.Identity

             // Using Mapster package to map the Object
             var product = request.Adapt<Product>();

             product.CreatedOn = DateTime.UtcNow;
             product.CreatedBy = userName;
             product.ModifiedBy = userName;
             product.ModifiedOn = DateTime.UtcNow;

             await repository.AddAsync(product);
             await repository.SaveChangesAsync();
             logger.LogInformation("Product created {productId}",product.Id);

             //return Created response with locatio header
             var location = $"/api/products/{product.Id}";
             return Results.Created(location, product); 
            
         })
         .WithName("CreateProduct")         
         .Produces<Product>(StatusCodes.Status201Created)
         .Produces(StatusCodes.Status400BadRequest)
         .Produces(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (int id, [FromBody] CreateProductRequest request,
                                                   [FromServices] IRepository<Product> repository,
                                                   [FromServices] IValidator<CreateProductRequest> validator,
                                                   [FromServices] ILogger<Product> logger,
                                                   CancellationToken cancellationToken) =>
        {
            var existingProduct = await repository.GetByIdAsync(id, cancellationToken);
            if (existingProduct == null)
            {
                return Results.NotFound(new Error("Product Not Found"));
            }
            request.Adapt(existingProduct);
            await repository.UpdateAsync(existingProduct, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Results.Ok(existingProduct);
        })
        .WithName("UpdateProduct");


        group.MapDelete("/{id}", async (int id, [FromServices] IRepository<Product> repository) =>
        {
            var product = await repository.GetByIdAsync(id);
            if (product is null)
            {
                return Results.NotFound();
            }
            await repository.DeleteAsync(product);
            return Results.NoContent();
        })
        .WithName("DeleteProduct");
        
    }
}
