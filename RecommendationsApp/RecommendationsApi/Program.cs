using Microsoft.Extensions.AI;
using MongoDB.Driver;
using RecommendationsApi.Articles;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<BlogService>();

builder.AddOllamaApiClient("voyage-4-nano")
       .AddEmbeddingGenerator(); // IEmbeddingGenerator

builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(builder.Configuration.GetConnectionString("mongo")));

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("embeddings/generate", async (BlogService blogService,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IMongoClient mongoClient,
    ILogger<Program> logger) =>
{
    var database = mongoClient.GetDatabase("recommendations");
    var collection = database.GetCollection<Article>("articles");

    var articles = new List<Article>();
    foreach(var articleUrl in File.ReadAllLines("sitemap_urls.txt"))
    {
        try
        {
            // Get Title and Content from the blogService
            var (title, content) = await blogService.GetTitleAndContentAsync(articleUrl);

            // Generate Embeddings for the content
            var embedding = await embeddingGenerator.GenerateAsync(content);

            // Store in Mongodb
            var article = new Article
            {
                Url = articleUrl,
                Title = title,
                Content = content,
                Embedding = embedding.Vector.ToArray()
            };
            articles.Add(article);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Generating embeddings failed for {ArticleUrl}:{Message}. Retrying..",
                articleUrl,
                ex.Message);
        }

    }
    await collection.InsertManyAsync(articles);
    
    return Results.Ok();
});


app.MapGet("article/{slug}/recommendations", async (
    string slug,
    BlogService blogService,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IMongoClient mongoClient) =>
{
    var articleUrl = File.ReadAllLines("sitemap_urls.txt").FirstOrDefault(a => a.EndsWith(slug));

    if (string.IsNullOrEmpty(articleUrl))
    {
        return Results.NotFound();
    }

    var (title, content) = await blogService.GetTitleAndContentAsync(articleUrl);

    var embedding = await embeddingGenerator.GenerateAsync(content);

    var vectorSearchOptions = new VectorSearchOptions<Article>
    {
        IndexName = "vector_index",
        //NumberOfCandidates = 10,
        Filter = Builders<Article>.Filter.Where(a=>a.Title != title),
        Exact = true,
    };
    var database = mongoClient.GetDatabase("recommendations");
    var collection = database.GetCollection<Article>("articles");

    var results = await collection.Aggregate()
    .VectorSearch(a => a.Embedding, embedding.Vector.ToArray(), 3, vectorSearchOptions)
    .ToListAsync();

    return Results.Ok(new
    {
        articleUrl,
        title,
        recommendations = results.Select(a => new
        { a.Title, a.Url }).ToArray()
    });

});

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

