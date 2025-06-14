namespace AppreciateAppApi.Services;

public interface IBlobStorageService
{
    Task<bool> UploadProfileAsync(string containerName, string blobName, Stream content);
    Task<string> GetBlob(string containerName, string blobName);
}
