using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Refit;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRefitClient<IBlogApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://BlogPostUrl"));

builder.AddOpenAIClient("ai-model");

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1"));


app.MapGet("/posts/{id}", async (int id, IBlogApi blogApi) =>
    await blogApi.GetPostAsync(id));

app.MapGet("/posts", async (int? userId, IBlogApi blogApi) =>
    await blogApi.GetPostsAsync(userId));

app.MapPost("/posts", async ([FromBody] Post post, IBlogApi blogApi) =>
    await blogApi.CreatePostAsync(post));

app.MapPut("/posts/{id}", async (int id, [FromBody] Post post, IBlogApi blogApi) =>
    await blogApi.UpdatePostAsync(id, post));

app.MapDelete("/posts/{id}", async (int id, IBlogApi blogApi) =>
    await blogApi.DeletePostAsync(id));

app.Run();

