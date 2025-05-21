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
}
