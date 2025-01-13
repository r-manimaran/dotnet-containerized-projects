using Azure.Refit.Api;
using Azure.Refit.Api.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRefitClient<IBlogApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"));

// Add OpenTelemetry Tracing data
// Telemetry data will collected in Jaeger running in docker
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Azure.Refit.Api"))
    .WithTracing(tracking =>
    {
        tracking
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        tracking.AddOtlpExporter();
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/posts", async (int? userId, IBlogApi api) =>
    await api.GetPostsAsync(userId));

app.MapGet("/posts/{id:int}", async(int id, IBlogApi api) =>
    await api.GetPostAsync(id));

app.MapPost("/posts", async ([FromBody] Post post, IBlogApi api) =>
    await api.CreatePostAsync(post));

app.MapPut("/posts/{id:int}", async(int id, [FromBody]Post post, IBlogApi api)=>
    await api.UpdatePostAsync(id,post));

app.MapDelete("/posts/{id:int}", async(int id, IBlogApi api)=>
    await api.DeletePostAsync(id));

//app.MapGet("/posts/{id:int}/comments", async(int id, IBlogApi api) =>
//    await api.GetPostsCommentsAsync(id));

app.Run();

