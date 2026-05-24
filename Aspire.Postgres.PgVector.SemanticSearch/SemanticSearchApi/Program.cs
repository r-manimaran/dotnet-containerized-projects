using Dapper;
using Microsoft.Extensions.AI;
using Npgsql;
using Pgvector;
using Pgvector.Dapper;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddOllamaApiClient("qwen3-embedding")
       .AddEmbeddingGenerator();

builder.AddNpgsqlDataSource("articles", configureDataSourceBuilder: b =>
{
    b.UseVector();
});

SqlMapper.AddTypeHandler(new VectorTypeHandler());

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapPost("/init", async (NpgsqlDataSource dataSource) =>
{
    await using var conn = await dataSource.OpenConnectionAsync();

    await using var enableExt = new NpgsqlCommand(
        "CREATE EXTENSION IF NOT EXISTS vector", conn);
    await enableExt.ExecuteNonQueryAsync();

    conn.ReloadTypes();

    await conn.ExecuteAsync(
        """
        CREATE TABLE IF NOT EXISTS articles (
            id SERIAL PRIMARY KEY,
            url TEXT NOT NULL,
            title TEXT NOT NULL,
            embedding vector(1024) NOT NULL
            )
        """);

    await conn.ExecuteAsync(
        """
        CREATE INDEX IF NOT EXISTS articles_embedding_idx ON articles USING hnsw (embedding vector_cosine_ops)
        """);

    return Results.Ok("Database initialized with pgvector extension and articles table.");
});

app.MapPost("/articles", async (NpgsqlDataSource dataSource,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<Program> logger) =>
{

    await using var conn = await dataSource.OpenConnectionAsync();
    conn.ReloadTypes();

    int count = 0;
    // 1 . Rad articles.json file and parse it into a list of Article objects.
    var articlesJson = await File.ReadAllTextAsync("articles.json");
    var articles = System.Text.Json.JsonSerializer.Deserialize<List<Article>>(articlesJson);

    foreach (var article in articles)
    {
        try
        {
            var embedding = await embeddingGenerator.GenerateAsync(article.Content);

            await conn.ExecuteAsync(
                "INSERT INTO articles (url, title, embedding) VALUES (@Url, @Title, @Embedding)",
                new { url = article.Url, title = article.Title, embedding = new Vector(embedding.Vector.ToArray()) });

            count++;
            logger.LogInformation("Processed article({count}) '{Title}' with URL '{Url}'",
                count, article.Title, article.Url);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error processing article '{article.Title}': {ex.Message}");
            continue; // Skip this article and continue with the next one.
        }
    }

    return Results.Ok("Article added successfully.");

});

app.MapPost("/search", async (NpgsqlDataSource dataSource,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    string query) =>
{
    await using var conn = await dataSource.OpenConnectionAsync();
    
    conn.ReloadTypes();

    var queryEmbedding = await embeddingGenerator.GenerateAsync(query);
    var embedding = new Vector(queryEmbedding.Vector.ToArray());


    var results = await conn.QueryAsync<SearchResult>(
        "SELECT url, title, embedding <=> @embedding as distance FROM articles ORDER BY embedding <=> @embedding LIMIT 5",
        new { embedding });

    return Results.Ok(new {query, results});
});


app.Run();

public record Article
{
    [JsonPropertyName("url")]
    public string Url { get; init; }
    [JsonPropertyName("title")]
    public string Title { get; init; }
    [JsonPropertyName("content")]
    public string Content { get; init; }
}

public record SearchResult
{
    public string Url { get; init; }

    public string Title { get; init; }

    public decimal Distance { get; init; }
}
