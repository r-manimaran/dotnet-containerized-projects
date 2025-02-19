using Aspire.Database.TestContainers.DTOs;
using Aspire.Database.TestContainers.Models;
using Aspire.Database.TestContainers.Testing.Abstractions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.Redis;

namespace Aspire.Database.TestContainers.Testing.Products;

public class CreateProductTests: BaseIntegrationTest
{
    public CreateProductTests(ApiFactory factory):base(factory)
    {
        
    }

    [Fact]
    public async Task CreateProduct_ShouldSucesss()
    {
        // Arrange
        var request = new Product
        {
            Id = 0,
            Name = _faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(_faker.Commerce.Price(1, 500, 2)),
            Quantity = _faker.Random.Number(1, 100)
        };

        // Act
        var response = await _apiClient.PostAsJsonAsync("api/Products/add",request);

        // Assert
        var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        
        Assert.NotNull(serviceResponse);
        serviceResponse.Success.Should().BeTrue();
        //Assert.True(serviceResponse.Success);
        Assert.NotNull(serviceResponse.Data);

        // Verify in Database
        var dbProduct = await _dbContext.Products.FindAsync(serviceResponse.Data.Id);
        dbProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProduct_ShouldReturnSavedProduct()
    {
        // Arrange
        var request = new Product
        {
            Id = 0,
            Name = _faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(_faker.Commerce.Price(1, 500, 2)),
            Quantity = _faker.Random.Number(1, 100)
        };

        // Act
        var createResponse = await _apiClient.PostAsJsonAsync("api/Products/add", request);
        var serviceResponse = await createResponse.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        var productId = serviceResponse.Data.Id;

        var getResponse = await _apiClient.GetAsync($"api/Products/{productId}");
        var getServiceReponse = await getResponse.Content.ReadFromJsonAsync<ServiceResponse<Product>>();

        // Assert
        Assert.NotNull(getServiceReponse);
        getServiceReponse.Success.Should().BeTrue();
        Assert.NotNull(getServiceReponse.Data);
        Assert.Equal(request.Name, serviceResponse.Data.Name);
        Assert.Equal(request.Quantity, serviceResponse.Data.Quantity);
        Assert.Equal(request.Price, serviceResponse.Data.Price);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnFromCache_WhenInCache()
    {
        // Arrange
        var request = new Product
        {
            Id = 0,
            Name = _faker.Commerce.ProductName(),
            Price = Convert.ToDecimal(_faker.Commerce.Price(1, 500, 2)),
            Quantity = _faker.Random.Number(1, 100)
        };

        var createResponse = await _apiClient.PostAsJsonAsync("api/Products/add", request);
        var serviceResponse = await createResponse.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        var productId = serviceResponse.Data.Id;
        
        // First call to cache the product
        var getResponse = await _apiClient.GetAsync($"api/Products/{productId}");
        

        // Act
        var result = await _apiClient.GetAsync($"api/Products/{productId}");

        var getServiceReponse = await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        // Assert
        Assert.NotNull(getServiceReponse);
        Assert.True(getServiceReponse.Success);
        Assert.Equal(" From Cache", getServiceReponse.Message);
        Assert.Equal(request.Name, getServiceReponse.Data.Name);
    }

    //[Fact]
    //public async Task TestRedisSetAndGet()
    //{
    //    var redisConnection = _redisContainer.GetConnectionString();
    //    var redis = ConnectionMultiplexer.Connect(redisConnection);
    //    var db = redis.GetDatabase();

    //    // Set a value in Redis
    //    await db.StringSetAsync("test-key", "test-value");

    //    // Get the value from Redis
    //    var value = await db.StringGetAsync("test-key");

    //    Assert.Equal("test-value", value);
    //}
}
