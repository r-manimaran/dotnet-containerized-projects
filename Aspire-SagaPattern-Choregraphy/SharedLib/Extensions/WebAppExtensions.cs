using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedLib.Extensions;

public static class WebAppExtensions
{
    public static async Task ApplyMigrationAsync<TContext>(this WebApplication app,
           bool seedData = false,
           Func<TContext,Task>? seedDataFunc = null)
         where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation($"Starting database Migration for {typeof(TContext).Name}");

            await context.Database.MigrateAsync();

            logger.LogInformation($"Database migration completed for {typeof(TContext).Name}");

            // optional seeding
            if(seedData && seedDataFunc != null)
            {
                logger.LogInformation($"Started Seeding data for {typeof(TContext).Name}");
                await seedDataFunc(context);
                logger.LogInformation($"Data seeding completed for {typeof(TContext).Name}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,$"An error occured while migrating/seeding the database for {typeof(TContext).Name}");
            throw;
        }

    }
}
