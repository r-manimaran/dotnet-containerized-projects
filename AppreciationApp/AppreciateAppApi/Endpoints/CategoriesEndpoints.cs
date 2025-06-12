using AppreciateAppApi.DTO;
using AppreciateAppApi.Services;

namespace AppreciateAppApi.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appreciation/categories").WithTags("Categories").RequireAuthorization();

        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Results.Ok(categories);
        });

        group.MapGet("/{id}", async (int id, ICategoryService categoryService) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return category != null ? Results.Ok(category) : Results.NotFound();
        });


        group.MapPost("/", async (CreateCategoryRequest category, ICategoryService categoryService) =>
        {
            var createdCategory = await categoryService.CreateCategoryAsync(category);
            return Results.Created($"/api/categories/{createdCategory.Data.Id}", createdCategory);
        });


        group.MapPut("/{id}", async (int id, CreateCategoryRequest category, ICategoryService categoryService) =>
        {
            var updatedCategory = await categoryService.UpdateCategoryAsync(id, category);
            return updatedCategory != null ? Results.Ok(updatedCategory) : Results.NotFound();
        });

        group.MapDelete("/{id}", async (int id, ICategoryService categoryService) =>
        {
            var deleted = await categoryService.DeleteCategoryAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

    }
}
