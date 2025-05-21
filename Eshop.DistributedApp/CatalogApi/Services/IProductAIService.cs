namespace CatalogApi.Services;

public interface IProductAIService
{
    Task<string> SupportAsync(string query);
}
