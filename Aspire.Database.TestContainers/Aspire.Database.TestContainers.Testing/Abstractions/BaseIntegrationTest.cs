using Aspire.Database.TestContainers.Data;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.Database.TestContainers.Testing.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<ApiFactory>, IDisposable
{
    protected readonly ApiFactory _factory;
    protected readonly AppDbContext _dbContext;
    protected readonly IServiceScope _scope;
    protected readonly HttpClient _apiClient;
    protected readonly IModel _channel;
    protected Faker _faker { get; }
    protected readonly RabbitMqConsumer _rabbitMqConsumer;
    protected BaseIntegrationTest(ApiFactory factory)
    {
        _factory = factory;
        _apiClient = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _faker = new Faker();
        _channel = factory.Services.GetRequiredService<IModel>();
        _rabbitMqConsumer = factory.RabbitMqConsumer;
    }
    public void Dispose()
    {
        _scope.Dispose();
    }
}
