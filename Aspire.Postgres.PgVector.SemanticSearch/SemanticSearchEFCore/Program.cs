using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Npgsql;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using SemanticSearchEFCore;
using SemanticSearchEFCore.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddOllamaApiClient("qwen3-embedding")
       .AddEmbeddingGenerator();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("articles-core"));
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(dataSource, x =>
{
    x.UseVector();
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapPost("/init", async (AppDbContext dbContext) =>
{
    await dbContext.Database.ExecuteSqlRawAsync(
        """
        CREATE EXTENSION IF NOT EXISTS vector
        """);

    await dbContext.Database.ExecuteSqlRawAsync(
        """
        CREATE TABLE IF NOT EXISTS articles (
            id SERIAL PRIMARY KEY,
            url TEXT NOT NULL,
            title TEXT NOT NULL,
            embedding vector(1024) NOT NULL
            )
        """);
    await dbContext.Database.ExecuteSqlRawAsync(
        """
        CREATE INDEX IF NOT EXISTS articles_embedding_idx ON articles USING hnsw (embedding vector_cosine_ops)
        """);

    return Results.Ok("Database initialized with pgvector extension and articles table.");
});
// Create the Migration and Update the database using the following commands in the terminal:
// dotnet ef migrations add InitialCreate -p SemanticSearchEFCore -s Aspire.Postgres.PgVector.SemanticSearch.AppHost


app.MapGet("/ingestData", async (AppDbContext dbContext, 
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<Program> logger) =>
{
    var articlesJson = await File.ReadAllTextAsync("articles.json");
    var articles = System.Text.Json.JsonSerializer.Deserialize<List<ArticleModel>>(articlesJson);

    foreach (var article in articles)
    {
        try
        {
            Article newArticle = new Article();
            newArticle.Title = article.Title;
            newArticle.Url = article.Url;
            newArticle.Content = article.Content;
            
            var embedding = await embeddingGenerator.GenerateAsync(article.Content);
            var embeddingArray = new Vector(embedding.Vector.ToArray());

            newArticle.Embedding = embeddingArray;

            dbContext.Articles.Add(newArticle);
            dbContext.SaveChanges();
        }
        catch (Exception ex) 
        {
            logger.LogError("Error in inserting Article" + ex.Message);
        }
    }
});


app.MapGet("/search", async (string query, AppDbContext dbContext, 
            IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator
    ) =>
{
    int limit = 5;
    var embeddings = await embeddingGenerator.GenerateAsync(query);
    var queryEmbedding = new Vector(embeddings.Vector);


    var embedding = new float[1024]; // Replace with actual embedding generation logic
    var results = await dbContext.Articles
    .Select(a => new
    {
        a.Title,
        a.Url,       
        Distance = a.Embedding.CosineDistance(queryEmbedding),
        a.Content
    })
        .OrderBy(a => a.Distance)
        .Take(limit)
        .ToListAsync();

    return Results.Ok(results);
});

app.Run();
