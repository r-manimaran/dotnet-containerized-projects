using Aspire.Redis.HybridCache.Clients;
using Aspire.Redis.HybridCache.Configuration;
using Aspire.Redis.HybridCache.Endpoints;
using Microsoft.Extensions.Caching.Hybrid;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi( options =>
{
    options.AddDocumentTransformer((document,context,cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "MOVIE DB API",
            Version = "1.0.0",
            Description =" This API contains the endpoints for OMDB"
        };

        document.Info.Contact = new()
        {
            Email = "mail.goggle.com",
            Name = "Manimaran",
            Url = new Uri("https://github.com/r-manimaran")
        };
        return Task.CompletedTask;
    });
});

// Read the Configuration from AppSettings
builder.Services.Configure<OmdbApiSettings>(builder.Configuration.GetSection("OmdbApi"));

// Create a named httpclient
builder.Services.AddHttpClient<OmdbApiClient>(client =>
{
    var settings = builder.Configuration.GetSection("OmdbApi").Get<OmdbApiSettings>();
    client.BaseAddress = new Uri(settings?.BaseUrl ?? throw new InvalidOperationException("OMDB API Base URL not set in configuration"));
});

builder.Services.AddMemoryCache();

#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(5)
    };
});
#pragma warning restore EXTEXP0018

builder.AddRedisDistributedCache("redis");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference(options =>
{
    options.Title = "Hybrid Cache";
    options.Theme = ScalarTheme.DeepSpace;
});

app.UseHttpsRedirection();

app.MapMovieEndpoints();

app.Run();


