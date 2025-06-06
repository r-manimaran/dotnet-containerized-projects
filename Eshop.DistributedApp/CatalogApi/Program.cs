using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<ProductDbContext>("catalogdb");

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IProductAIService, ProductAIService>();

builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.AddOllamaSharpChatClient("ollama-llama3-2");

builder.AddOllamaSharpEmbeddingGenerator("ollama-all-minilm");

//Register an in-memory vector store
builder.Services.AddInMemoryVectorStoreRecordCollection<int,ProductVector>("products");

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options => {
    options.SwaggerEndpoint(
    "/openapi/v1.json", "OpenAPI v1");
});

app.UseMigration();

app.UseHttpsRedirection();

app.MapProductsEndpoints();

app.Run();


