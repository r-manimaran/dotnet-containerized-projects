using MassTransit;

namespace BasketApi.Extensions;

public static class AppExtensions
{
    public static void MapMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(mt =>
        {
            mt.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("messaging", "/", host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });
            });
        });
    }
}
