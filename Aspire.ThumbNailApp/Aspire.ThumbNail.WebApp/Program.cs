using Aspire.ThumbNail.WebApp;
using Aspire.ThumbNail.WebApp.Components;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var config = builder.Configuration;
Console.WriteLine(config.GetConnectionString("blobs"));
Console.WriteLine(config.GetConnectionString("queues"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddAzureBlobServiceClient("blobs");

//builder.AddAzureQueueServiceClient("queues");

builder.Services.AddSingleton<QueueServiceClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("queues");
    
    // Clean the malformed connection string
    if (connectionString?.Contains(";;") == true)
    {
        connectionString = connectionString.Replace(";;", ";");
    }
    if (connectionString?.Contains("QueueName=") == true)
    {
        var parts = connectionString.Split(';');
        connectionString = string.Join(";", parts.Where(p => !p.StartsWith("QueueName=")));
    }
    
    return new QueueServiceClient(connectionString);
});

builder.Services.AddSingleton<QueueMessageHandler>();

builder.Services.AddHostedService<StorageWorker>();

builder.Services.AddSingleton(static provider => 
{
    try 
    {
        var queueServiceClient = provider.GetRequiredService<QueueServiceClient>();
        return queueServiceClient.GetQueueClient("thumbnail-queue");
    }
    catch (Exception ex)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to create QueueClient. Using fallback configuration.");
        
        // Fallback to direct connection string
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("queues") ?? "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";
        var fallbackClient = new QueueServiceClient(connectionString);
        return fallbackClient.GetQueueClient("thumbnail-queue");
    }
});

builder.Services.AddKeyedSingleton("images", 
    static (provider, _) => provider.GetRequiredService<BlobServiceClient>().GetBlobContainerClient("images"));

builder.Services.AddKeyedSingleton("thumbnails", 
    static (provider, _) => provider.GetRequiredService<BlobServiceClient>().GetBlobContainerClient("thumbnails"));

Console.WriteLine("All connection strings:");
foreach (var item in config.AsEnumerable())
{
    if (item.Key.Contains("ConnectionStrings"))
        Console.WriteLine($"{item.Key}: {item.Value}");
}

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
