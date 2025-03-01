
using Bogus;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Services;

public class DatabaseSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeeder> _logger;
    private const int BatchSize = 5000;
    private const int TotalRecords = 1_000_000;
    public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

   public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database is created with latest migrations
            await dbContext.Database.MigrateAsync(cancellationToken);

            // Check if we need to seed
            if (await dbContext.Products.AnyAsync(cancellationToken))
            {
                var count = await dbContext.Products.CountAsync(cancellationToken);
                _logger.LogInformation("Database already contains {Count} records, skipping seeding", count);
                return;
            }

            _logger.LogInformation("Starting database seeding");

            var processedCount = 0;
            
            while (processedCount < TotalRecords)
            {
                var batchSize = Math.Min(BatchSize, TotalRecords - processedCount);
                var records = GenerateRecords(batchSize).ToList();

                await dbContext.Products.AddRangeAsync(records, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                processedCount += batchSize;
                _logger.LogInformation("Processed {ProcessedCount} of {TotalRecords} records", processedCount, TotalRecords);

                // Clear the change tracker to prevent memory issues
                dbContext.ChangeTracker.Clear();
            }

            _logger.LogInformation("Database seeding completed. Total records: {Count}", processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static IEnumerable<Product> GenerateRecords(int count)
    {
        var faker = new Faker<Product>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
        .RuleFor(p => p.ProductId, f => Guid.NewGuid())
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.CreatedDate, f => f.Date.Past(2))
        .RuleFor(p => p.ManufactoringDate, f => f.Date.Past(3));

        return faker.Generate(count);                            
    }
}
