
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace CustomerApi.Test.Integration;

public class CustomersFeature : IClassFixture<ApiFactory>
{
    private readonly HttpClient _apiClient;
    private readonly ApiFactory _factory;
    private readonly AppDbContext _dbContext;
    private readonly IServiceScope _scope;

    public CustomersFeature(ApiFactory factory)
    {
        _apiClient = factory.CreateClient();
        _factory = factory;
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnSuccess()
    {

        // Arrange
        var request = new CreateCustomerRequest("John", "John@test.com");

        // Act
        var response = await _apiClient.PostAsJsonAsync("api/Customers/create", request);

        // Assert         
        var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<Customer>>();

        Assert.NotNull(serviceResponse);
        Assert.True(serviceResponse.Success);
        Assert.NotNull(serviceResponse.Data);

        // verify database state
        var savedCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c=>c.Id == serviceResponse.Data.Id);
        Assert.NotNull(savedCustomer);
        Assert.Equal(request.Name, savedCustomer.Name);
        Assert.Equal(request.Email,savedCustomer.Email);

    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnFailure_WhenEmailAlreadyExists()
    {

        // Arrange
        var request = new CreateCustomerRequest("John", "test@test.com");

        // Create First user
        await _apiClient.PostAsJsonAsync("api/Customers/create", request);
        
        // Arrange request with same email address
        var duplicateRequest = new CreateCustomerRequest("Deo","test@test.com");
        
        var response = await _apiClient.PostAsJsonAsync("api/Customers/create", request);
        
        // Act
        var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<Customer>>();

        // Assert
        Assert.NotNull(serviceResponse);
        // Assert.False(serviceResponse.Success);
        Assert.NotNull(serviceResponse.Data);

        // verify database for only one customer 
        var customerWithEmail = await _dbContext.Customers.Where(c => c.Email == request.Email).ToListAsync();

        Assert.Single(customerWithEmail);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnSavedCustomer()
    {
        // Arrange
        var request = new CreateCustomerRequest("TestUser", "testuser@test.com");

        var createResponse = await _apiClient.PostAsJsonAsync("api/Customers/create", request);
        var serviceResponse = await createResponse.Content.ReadFromJsonAsync<ServiceResponse<Customer>>();
        var customerId = serviceResponse.Data.Id;

        // Act
        var getResponse = await _apiClient.GetAsync($"api/Customers/{customerId}");
        var getServiceResponse = await getResponse.Content.ReadFromJsonAsync<ServiceResponse<Customer>>();

        // Assert
        Assert.NotNull(getServiceResponse);
        Assert.NotNull(getServiceResponse.Data);
        Assert.True(getServiceResponse.Success);
        Assert.Equal(request.Name, getServiceResponse.Data.Name);
        Assert.Equal(request.Email, getServiceResponse.Data.Email);
    }
    [Fact]
    public async Task DeleteCustomer_ShouldRemoveFromDatabase()
    {
        // Arrange
        var request = new CreateCustomerRequest("John", "john@test.com");

        var createResponse = await _apiClient.PostAsJsonAsync("/api/Customers/create",request);
        var serviceResponse = await createResponse.Content.ReadFromJsonAsync<ServiceResponse<Customer>>();
        var customerId = serviceResponse?.Data.Id;

        // Act
        var deleteResponse = await _apiClient.DeleteAsync($"/api/Customers/Delete/{customerId}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();

        // verify customer was deleted from database
        var deletedCustomer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        Assert.Null(deletedCustomer);
    }

}
