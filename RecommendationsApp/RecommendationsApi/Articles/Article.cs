using MongoDB.Bson.Serialization.Attributes;

namespace RecommendationsApi.Articles;

[BsonIgnoreExtraElements]
public class Article
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("url")]
    public string Url { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }

    [BsonElement("embedding")]
    public float[] Embedding { get; set; }
}
