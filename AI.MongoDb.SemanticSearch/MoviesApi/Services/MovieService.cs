using Microsoft.Extensions.AI;
using MongoDB.Driver;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _moviesCollection;

    private readonly IMongoCollection<Movie> _embMovieCollection;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    public MovieService(IMongoClient mongoClient, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        var database = mongoClient.GetDatabase("sample_mflix");

        _moviesCollection = database.GetCollection<Movie>("movies");

        _embMovieCollection = database.GetCollection<Movie>("embedded_movies");

        _embeddingGenerator = embeddingGenerator;
    }

    public async Task<List<Movie>> GetMovies(string? term = null, int limit = 10)
    {
        var filter = Builders<Movie>.Filter.Empty;

        if (!string.IsNullOrEmpty(term))
        {
            filter = Builders<Movie>.Filter
                        .Where(m => m.Title.ToLower().Contains(term.ToLower()));
        }
        return await _embMovieCollection
                .Find(filter)
                .Limit(limit)
                .ToListAsync();
    }

    public async Task<List<Movie>> GetMoviesAsync(string? term=null, int limit=10)
    {
        var filter = Builders<Movie>.Filter.Empty;

        if(!string.IsNullOrEmpty(term))
        {
            filter = Builders<Movie>.Filter
                        .Where(m => m.Title.ToLower().Contains(term.ToLower()));
        }

        var vectorEmbeddings = await GenerateEmbeddings(term);

        var vectorSearchoptions = new VectorSearchOptions<Movie>
        {
            IndexName = "vector_index_1",
            NumberOfCandidates = 200
        };

        //return await _moviesCollection
        //        .Find(filter)
        //        .Limit(limit)
        //        .ToListAsync();

        return await _embMovieCollection
                    .Aggregate()
                    .VectorSearch(movie => movie.PlotEmbedding, vectorEmbeddings, limit, vectorSearchoptions)
                    .ToListAsync();
    }

    public async Task UpdateMoviesAsync()
    {
        try {

            // Set WriteConcern on the collection
        var embMoviesWithWriteConcern = _embMovieCollection.WithWriteConcern(WriteConcern.WMajority);
        
        // First, clear the embedded_movies collection
        await embMoviesWithWriteConcern.DeleteManyAsync(Builders<Movie>.Filter.Empty);
        
         var movies = await _moviesCollection
                          .Find(Builders<Movie>.Filter.Empty)
                          .Limit(3500)
                          .ToListAsync();

          
        if (movies.Any())
        {
            // Create bulk write operations
            var bulkOps = new List<WriteModel<Movie>>();
            
            foreach (var movie in movies)
            {
                bulkOps.Add(new InsertOneModel<Movie>(movie));
            }

            // Execute bulk write in smaller batches
            const int batchSize = 1000;
            for (int i = 0; i < bulkOps.Count; i += batchSize)
            {
                var batch = bulkOps.Skip(i).Take(batchSize).ToList();

                var bulkWriteResult = await embMoviesWithWriteConcern.BulkWriteAsync(
                    batch,
                    new BulkWriteOptions 
                    { 
                        IsOrdered = false,                       
                    }
                );

                Console.WriteLine($"Batch {i/batchSize + 1}: Inserted {bulkWriteResult.InsertedCount} documents");
            }

            // Verify the insertion
            var countInEmbedded = await _embMovieCollection.CountDocumentsAsync(Builders<Movie>.Filter.Empty);

            Console.WriteLine($"Total documents in embedded_movies collection: {countInEmbedded}");
        }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
       
    }

    private async Task<float[]> GenerateEmbeddings(string plot)
    {
        var generatedEmbeddings = await _embeddingGenerator.GenerateAsync([plot]);

        var embedding = generatedEmbeddings.Single();

        return embedding.Vector.ToArray();
    }
}
