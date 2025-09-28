using Microsoft.EntityFrameworkCore;

namespace Aspire.AI.SQLServer.Extensions;

public static class AppExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
        
        dbContext.Database.EnsureCreated();
        
        dbContext.Database.Migrate();
    }
}
