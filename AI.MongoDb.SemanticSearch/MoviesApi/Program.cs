using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using MongoDB.Driver;
using MoviesApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("mongodb")));

builder.Services.AddSingleton<IMovieService, MovieService>();

builder.Services.AddEmbeddingGenerator(
    new OllamaEmbeddingGenerator(
                        builder.Configuration["Ollama:Url"]!, "mxbai-embed-large"));

// builder.Services.AddOpenApi();

//#pragma warning disable SKEXP0010
//builder.Services.AddOpenAITextEmbeddingGeneration(
//    modelId: "text-embedding-ada-002",          // Name of the embedding model, e.g. "text-embedding-ada-002".
//    apiKey: builder.Configuration["OpenAI:key"]!,
//    //orgId: "YOUR_ORG_ID",         // Optional organization id.
//    //serviceId: "YOUR_SERVICE_ID", // Optional; for targeting specific services within Semantic Kernel
//    dimensions: 1536              // Optional number of dimensions to generate embeddings with.
//);

//builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
//{
//#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//    var factory = sp.GetRequiredService<ITextEmbeddingGenerationService>();
//#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//    return factory;
//});

//builder.Services.AddEmbeddingGenerator(
//    new EmbeddingGenerationOptions());
builder.Services.AddHostedService<MovieEmbeddingGenerator>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/api/movies", async (IMovieService movieService, string? term = null, int limit = 10) =>
{
    var movies = await movieService.GetMoviesAsync(term, limit);
    return Results.Ok(movies);
});

app.MapGet("/api/movies/insert", async (IMovieService movieService) =>
{
    await movieService.UpdateMoviesAsync();
    return Results.Ok("Inserted");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
