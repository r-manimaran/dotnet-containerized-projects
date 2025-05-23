using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

namespace CatalogApi.Services;

public class ProductAIService(IChatClient chatClient, 
                              IEmbeddingGenerator<string,Embedding<float>> embeddingGenerator,
                              VectorStoreCollection<int, ProductVector> productVectorStoreCollection,
                               ProductDbContext dbContext,
                              ILogger<ProductAIService> logger) : IProductAIService
{
    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        // use EmbeddingGenerator to get the embedding of the query
        // Use InMemoryVectorStore to search for the most similar products
        // return the products after performing semantic search
        if(!await productVectorStoreCollection.CollectionExistsAsync())
        {
            logger.LogWarning("Product vector store collection does not exist.");
            await InitEmbeddingsAsync();
        }
        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

        var vectorSearchOptions = new VectorSearchOptions<ProductVector>
        {
            IncludeVectors = true,
            VectorProperty= x => x.Vector,
        };

        var results = productVectorStoreCollection.SearchAsync(queryEmbedding,top:3, options:vectorSearchOptions);
       List<Product> products = new List<Product>();
        await foreach (var result in results)
        {
            products.Add(new Product
            {
                Id = result.Record.Id,
                Name = result.Record.Name,
                Description = result.Record.Description,
                ImageUrl = result.Record.ImageUrl,
                Price = result.Record.Price
            });
        }
        return products;
    }

    private async Task InitEmbeddingsAsync()
    {
        await productVectorStoreCollection.EnsureCollectionExistsAsync();

        var products = await dbContext.Products.ToListAsync();
        foreach(var product in products)
        {
            var productInfo = $"[{ product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]. It is available at [{product.ImageUrl}]";
            var embedding = await embeddingGenerator.GenerateVectorAsync(product.Name);
            var productVector = new ProductVector
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Vector = await embeddingGenerator.GenerateVectorAsync(productInfo)
            };
            await productVectorStoreCollection.UpsertAsync(productVector);
        }
    }

    public async Task<string> SupportAsync(string query)
    {
        var systemPrompt = """
            You are a helpful assistant for a product catalog API.
            You always reply with a short and funny message. If you don't know the answer, say "I don't know".
            You are not allowed to give any other information. For any other question, explain about that you can answer on Products question alone".
            At the end, offer one of our products: Hiking Poles -$24, outdoor Rain Jacket -$49, or a Camping Tent -$99.
            Do not store memory of the chat conversation.
            """;

        var chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System,systemPrompt),
            new ChatMessage(ChatRole.User, query)
        };

        var resultPrompt = await chatClient.GetResponseAsync(chatHistory);
        return resultPrompt.Messages[0].Contents[0].ToString()!;
    }
}
