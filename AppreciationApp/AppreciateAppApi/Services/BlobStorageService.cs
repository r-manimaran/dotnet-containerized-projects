using Azure.Storage.Blobs;

namespace AppreciateAppApi.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;       
    }
    public async Task<bool> UploadProfileAsync(string containerName, string blobName, Stream content)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync();
        var blobClient = container.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, true);
        return true;
    }
    public async Task<string> GetBlob(string containerName, string blobName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = container.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }
}
