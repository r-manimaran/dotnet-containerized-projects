
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrderApiService.Endpoints;
using OrderApiService.Hubs;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddRedisDistributedCache("cache");

builder.Services.AddCors();

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCors(p=>p.SetIsOriginAllowed(_=>true).AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

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

app.MapOrdersEndpoints();

app.MapHub<OrderNotificationHub>("/orderNotification");

app.UseHttpsRedirection();

app.Run();

