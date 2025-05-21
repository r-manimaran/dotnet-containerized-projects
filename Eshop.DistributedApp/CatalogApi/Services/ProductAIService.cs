using Microsoft.Extensions.AI;

namespace CatalogApi.Services;

public class ProductAIService(IChatClient chatClient, ILogger<ProductAIService> logger) : IProductAIService
{
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

        var resultPrompt = await chatClient.CompleteAsync(chatHistory);
        return resultPrompt.Message.Contents[0].ToString()!;
    }
}
