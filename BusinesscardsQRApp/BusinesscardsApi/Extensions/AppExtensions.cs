using BusinesscardsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinesscardsApi.Extensions;

public static class AppExtensions
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            
            await using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                await dbContext.Database.MigrateAsync();
                await DatabaseSeedService.SeedAsync(dbContext);
            }

        }
    }
}
