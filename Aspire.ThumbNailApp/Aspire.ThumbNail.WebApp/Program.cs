using Aspire.ThumbNail.WebApp;
using Aspire.ThumbNail.WebApp.Components;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddAzureBlobServiceClient("blobs");

builder.AddAzureQueueServiceClient("queues");

builder.Services.AddSingleton<QueueMessageHandler>();

builder.Services.AddHostedService<StorageWorker>();

builder.Services.AddSingleton(static provider => provider.GetRequiredService<QueueServiceClient>().GetQueueClient("thumbnail-queue"));

builder.Services.AddKeyedSingleton("images", 
    static (provider, _) => provider.GetRequiredService<BlobServiceClient>().GetBlobContainerClient("images"));

builder.Services.AddKeyedSingleton("thumbnails", 
    static (provider, _) => provider.GetRequiredService<BlobServiceClient>().GetBlobContainerClient("thumbnails"));

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
