using Carter;

namespace Orders.Api.Extensions;

public static class DIConfiguration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddEndpointsApiExplorer()
            .AddCarter()
            .AddSwaggerGen()
            .AddRequestTimeouts();
    }
}
