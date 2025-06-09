namespace AppreciateAppApi.Services;

using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using Microsoft.EntityFrameworkCore;

public class CategoryService(AppDbContext dbContext, 
                            ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Category created: {CategoryName}", category.Name);
        return category;
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

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        var categories = await dbContext.Categories
                            .AsNoTracking()
                            .ToListAsync();
        logger.LogInformation("Retrieved {Count} categories", categories.Count);
        return categories;
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        var existingCategory = await dbContext.Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);
       if (existingCategory == null)
       {
           logger.LogWarning("Category with ID {Id} not found", id);
           return null;
        }
        logger.LogInformation("Retrieved category with ID {Id}", id);
        return existingCategory;
    }

    public async Task<Category?> UpdateCategoryAsync(int id, CreateCategoryRequest request)
    {
        var existingCategory = await dbContext.Categories
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCategory == null)
        {
            logger.LogWarning("Category with ID {Id} not found for update", id);
            return null;
        }
        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;
        existingCategory.ImageUrl = request.ImageUrl;
        dbContext.Categories.Update(existingCategory);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Updated category with ID {Id}", id);
        return existingCategory;
    }
}
