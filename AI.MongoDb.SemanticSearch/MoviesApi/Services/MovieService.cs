using MongoDB.Driver;
using MoviesApi.Models;

namespace MoviesApi.Services;

public class MovieService : IMovieService
{
    private readonly IMongoCollection<Movie> _moviesCollection;

    private readonly IMongoCollection<Movie> _embMovieCollection;
    public MovieService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("sample_mflix");

        _moviesCollection = database.GetCollection<Movie>("movies");

        _embMovieCollection = database.GetCollection<Movie>("embedded_movies");
    }
    public async Task<List<Movie>> GetMoviesAsync(string? term=null, int limit=10)
    {
        var filter = Builders<Movie>.Filter.Empty;

        if(!string.IsNullOrEmpty(term))
        {
            filter = Builders<Movie>.Filter
                        .Where(m => m.Title.ToLower().Contains(term.ToLower()));
        }

        return await _moviesCollection
                .Find(filter)
                .Limit(limit)
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
}
