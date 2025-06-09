namespace AppreciateAppApi.Services;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;

public interface ICategoryService
{
    Task<BaseResponse<IEnumerable<Category>>> GetAllCategoriesAsync();
    Task<BaseResponse<Category?>> GetCategoryByIdAsync(int id);
    Task<BaseResponse<Category>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<BaseResponse<Category?>> UpdateCategoryAsync(int id, CreateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(int id);
}
