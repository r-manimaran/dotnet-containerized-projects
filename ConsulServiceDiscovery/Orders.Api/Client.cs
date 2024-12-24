using Contracts;

namespace Orders.Api;

public sealed class Client
{
    private readonly HttpClient _httpClient;

    public Client(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<Response?> GetAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<Response>($"api/Report/{id}");
        return response;
    }
}
