
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Embeddings;
using MongoDB.Driver;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class MovieEmbeddingGenerator : BackgroundService
{
    private readonly IMovieService _movieService;
    private readonly IMongoClient _mongoClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    //private readonly ITextEmbeddingGenerationService _openaiEmbeddingGenerator;
    private readonly ILogger<MovieEmbeddingGenerator> _logger;

    public MovieEmbeddingGenerator(IMovieService movieService, 
                                    IMongoClient mongoClient, 
                                    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
#pragma warning disable IDE0060 // Remove unused parameter
                                    //ITextEmbeddingGenerationService openaiEmbeddingGenerator,
#pragma warning restore IDE0060 // Remove unused parameter
                                    ILogger<MovieEmbeddingGenerator> logger)
    {
        _movieService = movieService;
        _mongoClient = mongoClient;
        _embeddingGenerator = embeddingGenerator;
       // this.openaiEmbeddingGenerator = openaiEmbeddingGenerator;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var database = _mongoClient.GetDatabase("sample_mflix");
        var movieCollection = database.GetCollection<Movie>("embedded_movies");

        try
        {
            var movies = await _movieService.GetMovies(limit: 10000);
            if (!movies.Any())
            {
                return;
            }

            var moviesWithPlots = movies
                .Where(m => !string.IsNullOrEmpty(m.Plot))
                .Where(m => m.PlotEmbedding is null or { Length: 0 })
                .ToList();
            var embeddings = new Dictionary<string, float[]>();
            foreach (var movie in moviesWithPlots)
            {
                if(!embeddings.ContainsKey(movie.Plot))
                {
                    try
                    {
                        embeddings[movie.Id] = await GeneratedEmbeddings(movie.Plot);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
            }

            // create bulk upate operation
            var updates = new List<UpdateOneModel<Movie>>();

            foreach (var movie in moviesWithPlots)
            {
                var filter = Builders<Movie>.Filter.Eq(m => m.Id, movie.Id);
                var update = Builders<Movie>.Update.Set(m => m.PlotEmbedding, embeddings[movie.Id]);
                updates.Add(new UpdateOneModel<Movie>(filter, update));
            }

            // Execute bulk update
            if(updates.Any())
            {
                await movieCollection.BulkWriteAsync(updates, cancellationToken: stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error processing batch:{Message}", ex.Message);
        }
    }

    private async Task<float[]> GeneratedEmbeddings(string plot)
    {
        var generatedEmbeddings = await _embeddingGenerator.GenerateAsync([plot]);

        var embedding = generatedEmbeddings.Single();

        return embedding.Vector.ToArray();
    }


    // Embeddings using OpenAI
    // private async Task<IList<ReadOnlyMemory<float>>> GenerateOpenAIEmbeddings(string plot)
    // {
        
    // }
}
