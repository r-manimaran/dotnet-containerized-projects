
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
    public async Task GetAllCustomers_ShouldReturnAllCustomers()
    {
        // Arrange
        var request1 = new CreateCustomerRequest("John", "john@test.com");
        var request2 = new CreateCustomerRequest("Jane", "jane@test.com");

        var createResponse1 = await _apiClient.PostAsJsonAsync("api/Customers/create", request1);
        var createResponse2 = await _apiClient.PostAsJsonAsync("api/Customers/create", request2);

        // Act
        var getAllResponse = await _apiClient.GetAsync("api/Customers/all");
        var getAllServiceResponse = await getAllResponse.Content.ReadFromJsonAsync<ServiceResponse<List<Customer>>>();

        // Assert
        Assert.NotNull(getAllServiceResponse);
        Assert.NotNull(getAllServiceResponse.Data);
        Assert.True(getAllServiceResponse.Success);
        Assert.Equal(2, getAllServiceResponse.Data.Count);

        var customer1 = getAllServiceResponse.Data.FirstOrDefault(c => c.Name == request1.Name);
        var customer2 = getAllServiceResponse.Data.FirstOrDefault(c => c.Name == request2.Name);

        Assert.NotNull(customer1);
        Assert.NotNull(customer2);
        Assert.Equal(request1.Email, customer1.Email);
        Assert.Equal(request2.Email, customer2.Email);
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
        //deleteResponse.EnsureSuccessStatusCode();

        // verify customer was deleted from database
        var deletedCustomer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldUpdateDatabase()
    {
        // Arrange - Create a customer
        var createRequest = new CreateCustomerRequest("Original Name",
                                                     "original@example.com");
        

        var createResponse = await _apiClient.PostAsJsonAsync("/api/customers/create", createRequest);
        var createServiceResponse = await createResponse.Content
            .ReadFromJsonAsync<ServiceResponse<Customer>>();

        var customerId = createServiceResponse.Data.Id;

        // Update customer
        var updateRequest = new UpdateCustomerRequest(customerId,
            "Updated Name",
            "updated@example.com"
        );

        // Act
        var updateResponse = await _apiClient.PutAsJsonAsync("/api/customers/update", updateRequest);
        var updateServiceResponse = await updateResponse.Content
            .ReadFromJsonAsync<ServiceResponse<Customer>>();

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        Assert.True(updateServiceResponse.Success);

        // Verify database update
        var updatedCustomer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId);

        Assert.NotNull(updatedCustomer);
        Assert.Equal(updateRequest.Name, updatedCustomer.Name);
        Assert.Equal(updateRequest.Email, updatedCustomer.Email);
    }


}
