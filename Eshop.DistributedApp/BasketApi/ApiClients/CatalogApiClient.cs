using CatalogApi.Models;

namespace BasketApi.ApiClients;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<Product> GetProductById(int id)
    {
        var response = await httpClient.GetFromJsonAsync<Product>($"/api/products/{id}");
        return response!;
    }
}
