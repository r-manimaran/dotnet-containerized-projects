using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aspire.ThumbNail.Shared;
using Aspire.ThumbNail.Shared.Serialization;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AzFuncThumbNailGenerator;

public class ThumbnailGenerator
{
    private readonly ILogger<ThumbnailGenerator> _logger;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly QueueClient _resultsQueueClient;
    private const int TargetHeight = 128;
    public ThumbnailGenerator(BlobServiceClient blobServiceClient, QueueServiceClient resultsQueueClient, ILogger<ThumbnailGenerator> logger)
    {
        _logger = logger;
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("");
        _resultsQueueClient = resultsQueueClient.GetQueueClient("thumbnail-queue");
    }

    [Function(nameof(ThumbnailGenerator))]
    public async Task RunResize([BlobTrigger("images/{name}", Connection = "blobs")] Stream stream, string name)
    {
        try
        {
            using var resizedStream = GetResizedImageStream(name, stream, SKEncodedImageFormat.Jpeg);

            await UploadResizedImageAsync(name, resizedStream);

            await SendQueueMessageAsync(name);

            _logger.LogInformation("Processed the uploaded image {name} successfully", name);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error Occured while uploading the image {name}: Exceptin:{Exception}", name, ex.ToString());
        }
    }

    private MemoryStream GetResizedImageStream(string name, Stream stream, SKEncodedImageFormat format)
    {
        using var originalBitmap = SKBitmap.Decode(stream);

        var scale =(float)TargetHeight / originalBitmap.Height;
        var targetWidth = (int)(originalBitmap.Width * scale);

        using var resizedBitmap = originalBitmap.Resize(
            new SKImageInfo(targetWidth, TargetHeight), new SKSamplingOptions(SKCubicResampler.Mitchell));

        using var image = SKImage.FromBitmap(resizedBitmap);

        var resizedStream = new MemoryStream();
        
        image.Encode(format,100).SaveTo(resizedStream);

        _logger.LogInformation("Resized image {Name} from {OriginalWidth}x{OriginalHeight} to {width}x{Height}.",
            name, originalBitmap.Width, originalBitmap.Height,
            targetWidth,TargetHeight);

        return resizedStream;
    }

    private async Task UploadResizedImageAsync(string name, MemoryStream resizedStream)
    {
        resizedStream.Position = 0;
        
        var blobClient = _blobContainerClient.GetBlobClient(name);
        
        _logger.LogDebug("Uploading {Name}", name);

        await blobClient.UploadAsync(resizedStream, overwrite: true);

        _logger.LogInformation("Upload {Name}", name);
    }

    private async Task SendQueueMessageAsync(string name)
    {
        var jsonMessage = JsonSerializer.Serialize(
            new UploadResult(name, true), SerializationContext.Default.UploadResult);

        _logger.LogDebug("Signaling upload of {Name}", name);
        
        await _resultsQueueClient.SendMessageAsync(jsonMessage);

        _logger.LogInformation("Signaled upload of {Name}", name);
    }
}