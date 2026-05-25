using System.Text.Json.Serialization;

namespace SemanticSearchEFCore;

public record ArticleModel
{
    [JsonPropertyName("url")]
    public string Url { get; init; }
    [JsonPropertyName("title")]
    public string Title { get; init; }
    [JsonPropertyName("content")]
    public string Content { get; init; }
}
