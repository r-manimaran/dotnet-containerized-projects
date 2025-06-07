using AppreciateAppApi.Data;
using AppreciateAppApi.Services;
using Microsoft.EntityFrameworkCore;

namespace AppreciateAppApi.Extensions;

public static class AppExtensions
{
    public static async Task ApplyMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            await dbContext.Database.MigrateAsync();
            // Seed the database with initial data if needed
            await DatabaseSeedService.SeedDatabaseAsync(dbContext);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
            logger.LogError(ex, "An error occurred while applying migrations.");
            throw;
        }
    }
}
