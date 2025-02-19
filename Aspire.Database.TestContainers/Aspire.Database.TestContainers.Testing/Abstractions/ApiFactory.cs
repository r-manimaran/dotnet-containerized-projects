using Aspire.Database.TestContainers.Data;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Aspire.Database.TestContainers.Testing.Abstractions;

public class ApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private const string RabbitMqUser = "guest";
    private const string RabbitMqPassword = "guest";
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("products")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage("redis:7.0").Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
                                    .WithImage("rabbitmq:3.12-management") 
                                    .WithEnvironment("RABBITMQ_DEFAULT_USER", RabbitMqUser)
                                    .WithEnvironment("RABBITMQ_DEFAULT_PASS", RabbitMqPassword)                                   
                                    .WithPortBinding(5672, 5672)
                                    .WithPortBinding(15672, 15672)
                                    .WithWaitStrategy(Wait.ForUnixContainer()                                    
                                        .UntilPortIsAvailable(5672)
                                        .UntilMessageIsLogged("Server startup complete."))
                                    .Build();

     private IConnection _rabbitConnection;
    public RabbitMqConsumer RabbitMqConsumer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var redisConnectionString = $"{_redisContainer.Hostname}:{_redisContainer.GetMappedPublicPort(6379)}";
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
                options
                       .UseNpgsql(_dbContainer.GetConnectionString())                   
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


            
            services.RemoveAll(typeof(IDistributedCache));
            services.RemoveAll(typeof(IConnectionMultiplexer));

             // Register IConnectionMultiplexer
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            
            services.AddStackExchangeRedisCache(redisCacheOptions =>
                         redisCacheOptions.Configuration = _redisContainer.GetConnectionString());


            services.AddSingleton<IConnection>(sp => 
            {
                var factory = new ConnectionFactory
                {
                    HostName =_rabbitMqContainer.Hostname,
                    Port = _rabbitMqContainer.GetMappedPublicPort(5672),
                    UserName ="guest",
                    Password = "guest",
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),                    
                };
              return factory.CreateConnection();
            });

            services.AddSingleton<IModel>(sp => {
                var connection = sp.GetRequiredService<IConnection>();
                return connection.CreateModel();
            });

        });
        //var connectionFactory = new ConnectionFactory();
        //connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());
        //_rabbitConnection = connectionFactory.CreateConnection();
         builder.ConfigureAppConfiguration((context, config) =>
        {
            var redisConnectionString = $"{_redisContainer.GetConnectionString()}";
            var configValues = new Dictionary<string, string>
            {
                { "ConnectionStrings:redis", redisConnectionString },
                { "Aspire:StackExchange:Redis:ConnectionString", redisConnectionString }
            };

            config.AddInMemoryCollection(configValues);
        });

        base.ConfigureWebHost(builder);
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
         await _redisContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        
        await WaitForRedisAsync();
        RabbitMqConsumer = new RabbitMqConsumer(_rabbitMqContainer.GetConnectionString());
        
    }

        private async Task WaitForRedisAsync()
    {
        var attempts = 0;
        const int maxAttempts = 30;
        const int delayMs = 1000;

        while (attempts < maxAttempts)
        {
            try
            {
                using var redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
                var db = redis.GetDatabase();
                await db.PingAsync();
                return;
            }
            catch
            {
                attempts++;
                if (attempts == maxAttempts)
                {
                    throw new Exception($"Redis failed to become ready after {maxAttempts} attempts");
                }
                await Task.Delay(delayMs);
            }
        }
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();

        if(_rabbitConnection?.IsOpen ?? false)
        {
            _rabbitConnection.Close();
            _rabbitConnection.Dispose();
        }
        
        await _rabbitMqContainer.StopAsync();
    }
}
