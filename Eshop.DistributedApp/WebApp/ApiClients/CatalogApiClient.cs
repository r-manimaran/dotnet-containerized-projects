using CatalogApi.Models;

namespace WebApp.ApiClients;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<List<Product>> GetProducts() //int page, int pageSize
    {
        var response = await httpClient.GetFromJsonAsync<List<Product>>($"/api/products"); 

            // $"//?page={page}&pageSize={pageSize}");
        
        return response!;
    }
    public async Task<Product> GetProductById(int id)
    {
        var response = await httpClient.GetFromJsonAsync<Product>($"/api/products/{id}");
        return response!;
    }

    public async Task<string> SupportProducts(string query)
    {
        var response = await httpClient.GetFromJsonAsync<string>($"/api/products/support/{query}");
        return response!;
    }

    public async Task<List<Product>> SearchProducts(string query, bool isAISearch)
    {
        if(isAISearch)
        {
            return await httpClient.GetFromJsonAsync<List<Product>>($"/api/products/aisearch/{query}");
        }
        else
        {
            return await httpClient.GetFromJsonAsync<List<Product>>($"/api/products/search/{query}");
        }         
        
    }
}
