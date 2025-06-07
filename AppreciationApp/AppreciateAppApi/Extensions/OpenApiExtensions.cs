using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AppreciateAppApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Appreciation App API",
                Version = "v1",
                Description = "API for the Appreciation App"
            });
        });

        return services;
    }

    public static WebApplication UseOpenApi(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Appreciation App API v1");
        });

        return app;
    }
}