﻿using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Eshop.DistributedApp.ServiceDefaults;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithAssemblies(
        this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.SetInMemorySagaRepositoryProvider();

            config.AddConsumers(assemblies);
            config.AddSagaStateMachines(assemblies);
            config.AddActivities(assemblies);

            config.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("rabbitmq");
                if (connectionString != null)
                {
                    configurator.Host(connectionString);
                    configurator.ConfigureEndpoints(context);
                }
            });
        });
        return services;
    }
}
