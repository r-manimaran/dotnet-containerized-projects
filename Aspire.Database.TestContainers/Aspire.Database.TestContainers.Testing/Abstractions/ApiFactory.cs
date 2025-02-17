using Aspire.Database.TestContainers.Data;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Aspire.Database.TestContainers.Testing.Abstractions;

public class ApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("products")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage("redis:7.0").Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
                                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
                                    .Build();

    //private IConnection _rabbitConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
                options
                       .UseNpgsql(_dbContainer.GetConnectionString())                   
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


            
            //services.RemoveAll(typeof(RedisCacheOptions));

            //services.AddStackExchangeRedisCache(redisCacheOptions =>
            //redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

        });
        //var connectionFactory = new ConnectionFactory();
        //connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());
        //_rabbitConnection = connectionFactory.CreateConnection();

        base.ConfigureWebHost(builder);
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        // await _redisContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        // await _redisContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }
}
