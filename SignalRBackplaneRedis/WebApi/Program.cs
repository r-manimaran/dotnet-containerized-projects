using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Trace;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApi.Helpers;
using WebApi.Hubs;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisDistributedCache("cache");

builder.Services.AddCors();

builder.Services.AddSignalR()
                .AddStackExchangeRedis(builder.Configuration.GetConnectionString("cache")!)
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddOpenApi();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCors(p => p.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

var instanceId = Environment.GetEnvironmentVariable("OTEL_RESOURCE_ATTRIBUTES")
    ?.Split(",")
    .Select(attr => attr.Split('=', 2))
    .FirstOrDefault(parts => parts is ["service.instance.id", _])
    ?[1] ?? Environment.MachineName;

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Instance-Id"] = instanceId;
        return Task.CompletedTask;
    });
    await next();
});

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options => {
    options.SwaggerEndpoint(
    "/openapi/v1.json", "OpenAPI v1");
});
app.UseAuthorization();

// API Endpoints
/// Create the Order
app.MapPost("orders", async (IDistributedCache cache, ClaimsPrincipal claimsPrincipal) =>
{
    var order = DummyDataGenerator.GenerateDummyOrder();
    order.UserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    order.StatusTransitions.Add(new StatusTransition
    {
        Status = order.Status,
        Timestamp = order.CreatedAt,
        InstanceId = instanceId
    });
    var cacheKey = $"order:{order.Id}";

    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };
    await cache.SetStringAsync(cacheKey,
        JsonSerializer.Serialize(order),
        options);

    return Results.Created($"orders/{order.Id}", order);
}).RequireAuthorization();


app.MapPut("orders/{id}", async (
    Guid id,
    OrderStatus status,
    IDistributedCache cache,
    IHubContext<OrderNotificationHub, IOrderNotificationClient> hubContext) =>
{
    if(!Enum.IsDefined(typeof(OrderStatus), status))
    {
        return Results.BadRequest(new { error = $"Invalid order status:{status}" });
    }

    var cacheKey = $"order:{id}";
    var orderJson = await cache.GetStringAsync(cacheKey);

    if(orderJson == null)
    {
        return Results.NotFound();
    }
     var order = JsonSerializer.Deserialize<Order>(orderJson);
    if (order == null)
    {
        return Results.NotFound();
    }
    order.Status = status;
    order.UpdatedAt = DateTime.UtcNow;
    order.StatusTransitions.Add(new StatusTransition
    {
        Status = status,
        Timestamp = order.UpdatedAt,
        InstanceId = instanceId
    });

    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };

    await cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(order),
        options);
    await hubContext.Clients.User(order.UserId!).OrderStatusUpdated(order, instanceId);
    return Results.Ok(order);
});

app.MapGet("orders/{id}", async (Guid id, IDistributedCache cache) =>
{
    var cacheKey = $"order:{id}";
    var orderJson = await cache.GetStringAsync(cacheKey);
    if (orderJson == null)
    {
        return Results.NotFound();
    }

    var order = JsonSerializer.Deserialize<Order>(orderJson);
    return Results.Ok(order);
});

app.MapHub<OrderNotificationHub>("/orderNotification");

app.UseHttpsRedirection();

app.Run();

