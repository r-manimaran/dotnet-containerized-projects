using Microsoft.EntityFrameworkCore;
using Products.Api.Database;

namespace Products.Api.Extensions;

public static class DbExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
