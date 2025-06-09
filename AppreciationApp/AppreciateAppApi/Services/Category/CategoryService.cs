namespace AppreciateAppApi.Services;

using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using Microsoft.EntityFrameworkCore;

public class CategoryService(AppDbContext dbContext, 
                            ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<BaseResponse<Category>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var response = new BaseResponse<Category>();

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Category created: {CategoryName}", category.Name);
        response.Data = category;
        response.Success = true;
        response.Message = "Category created successfully.";

        return response;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var existingCategory = await dbContext.Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCategory == null)
            {
            logger.LogWarning("Category with ID {Id} not found for deletion", id);
            return false;
        }
        dbContext.Categories.Remove(existingCategory);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Deleted category with ID {Id}", id);
        return true;
    }

    public async Task<BaseResponse<IEnumerable<Category>>> GetAllCategoriesAsync()
    {
        var response = new BaseResponse<IEnumerable<Category>>();

        var categories = await dbContext.Categories
                            .AsNoTracking()
                            .ToListAsync();
        logger.LogInformation("Retrieved {Count} categories", categories.Count);
        response.Data = categories;
        response.Success = true;
        response.Message = "Categories retrieved successfully.";
        return response;
    }

    public async Task<BaseResponse<Category?>> GetCategoryByIdAsync(int id)
    {
        var response = new BaseResponse<Category?>();

        var existingCategory = await dbContext.Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);

       if (existingCategory == null)
       {
           logger.LogWarning("Category with ID {Id} not found", id);
            response.Success = false;
            response.Message = "Category not found.";
            return response;
        }

        logger.LogInformation("Retrieved category with ID {Id}", id);
        response.Data = existingCategory;
        response.Message = "Category retrieved successfully.";
        response.Success = true;
        
        return response;
    }

    public async Task<BaseResponse<Category?>> UpdateCategoryAsync(int id, CreateCategoryRequest request)
    {
        var response = new BaseResponse<Category?>();
        var existingCategory = await dbContext.Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCategory == null)
        {
            logger.LogWarning("Category with ID {Id} not found for update", id);
            response.Success = false;
            response.Message = "Category not found.";
            return response;
        }
        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;
        existingCategory.ImageUrl = request.ImageUrl;
        dbContext.Categories.Update(existingCategory);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Updated category with ID {Id}", id);
        response.Data = existingCategory;
        response.Success = true;
        response.Message = "Category updated successfully.";
        return response;
    }
}
