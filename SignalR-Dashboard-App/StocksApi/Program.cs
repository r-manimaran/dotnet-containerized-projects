using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StocksApi;
using StocksApi.Endpoints;
using StocksApi.Realtime;
using StocksApi.Stocks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

builder.Services.AddCors();

builder.Services.AddSignalR();

builder.Services.AddSingleton(_ =>
{
    string connectionString = builder.Configuration.GetConnectionString("Database")!;

    var npgsqlDataSource = NpgsqlDataSource.Create(connectionString);

    return npgsqlDataSource;
});

builder.Services.Configure<StockUpdateOptions>(builder.Configuration.GetSection("StockUpdateOptions"));

builder.Services.AddHostedService<DatabaseInitializer>();

builder.Services.AddHttpClient<StocksClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["Stocks:ApiUrl"]!);
});

builder.Services.AddScoped<StockService>();

builder.Services.AddSingleton<ActiveTickerManager>();

builder.Services.AddHostedService<StocksFeedUpdater>();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(r => r.AddService("StocksApi"))
       .WithTracing(tracing =>
            tracing.
                   AddNpgsql()
                   .AddHttpClientInstrumentation()
                   .AddAspNetCoreInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation())
       .UseOtlpExporter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(opt =>
        opt.SwaggerEndpoint("/openapi/v1.json", "openapi v1"));
}
app.MapStocksEndpoints();

app.UseCors(policy => policy
    .WithOrigins(builder.Configuration["Cors:AllowedOrigin"]!)
    .AllowCredentials()
    .AllowAnyHeader()
    .AllowAnyMethod()
    );
app.MapHub<StocksFeedHub>("/stocks-feed");
app.UseHttpsRedirection();

app.Run();


