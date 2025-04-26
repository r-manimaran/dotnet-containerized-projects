using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoviesApi.Models;

[BsonIgnoreExtraElements]
public class Movie
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("plot")]
    public string Plot { get; set; }
    [BsonElement("genres")]
    public List<string> Genres { get; set; }
    [BsonElement("runtime")]
    public int Runtime { get; set; }
    [BsonElement("cast")]
    public List<string> Cast { get; set; }
    [BsonElement("num_mflix_comments")]
    public int NumMflixComments { get; set; }
    [BsonElement("poster")]
    public string Poster { get; set; }
    [BsonElement("fullplot")]
    public string FullPlot { get; set; }
    [BsonElement("title")]
    public string Title { get; set; }
    [BsonElement("type")]
    public string Type { get; set; }
    public Tomatoes Tomatoes { get; set; }
    [BsonElement("metacritic")]
    public int? Metacritic { get; set; }
    [BsonElement("awesome")]
    public bool? Awesome { get; set; }
    [BsonElement("lastupdated")]
    public string LastUpdated { get; set; }
    [BsonElement("year")]
    public int Year { get; set; }
    [BsonElement("awards")]
    public Awards Awards { get; set; }

    [BsonElement("imdb")]
    public Imdb Imdb { get; set; }
    [BsonElement("directors")]
    public List<string> Directors { get; set; }
    [BsonElement("writers")]
    public List<string> Writers { get; set; }
    [BsonElement("countries")]
    public List<string> Countries { get; set; }
    [BsonElement("languages")]
    public List<string> Languages { get; set; }
}

[BsonIgnoreExtraElements]
public class Awards
{
    [BsonElement("wins")]
    public int Wins { get; set; }
    [BsonElement("nominations")]
    public int Nominations { get; set; }
    [BsonElement("text")]
    public string Text { get; set; }
}

[BsonIgnoreExtraElements]
public class Imdb
{
    [BsonElement("rating")]
    public decimal Rating { get; set; }
    [BsonElement("votes")]
    public int Votes { get; set; }

    //[BsonIgnore]
    [BsonElement("id")]
    public int id { get; set; }
}
[BsonIgnoreExtraElements]
public class Tomatoes
{
    [BsonElement("viewer")]
    public Viewer Viewer { get; set; }
    [BsonElement("fresh")]
    public int Fresh { get; set; }
    [BsonElement("critic")]
    public Critic Critic { get; set; }
    [BsonElement("rotten")]
    public int Rotten { get; set; }
    [BsonElement("lastupdated")]
    public DateTime LastUpdated { get; set; }
}

[BsonIgnoreExtraElements]

public class Viewer
{
    [BsonElement("rating")]
    public float Rating { get; set; }
    [BsonElement("numReviews")]
    public int NumReviews { get; set; }
    [BsonElement("meter")]
    public int Meter { get; set; }
}

[BsonIgnoreExtraElements]
public class Critic
{
    [BsonElement("rating")]
    public float Rating { get; set; }
    [BsonElement("numReviews")]
    public int NumReviews { get; set; }
    [BsonElement("meter")]
    public int Meter { get; set; }
}
