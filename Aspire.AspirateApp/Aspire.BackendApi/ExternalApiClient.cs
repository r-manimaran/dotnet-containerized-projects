namespace Aspire.BackendApi;

public class ExternalApiClient(HttpClient client)
{
    public async Task SendHit(string text)
    {
       
        await client.GetAsync($"bbee41fc-829e-4b3d-bd53-5492a325136b/?data={text}");
    }
}
