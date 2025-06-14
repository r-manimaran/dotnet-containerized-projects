using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace AppreciateAppApi.Services;

public class EmployeeService(AppDbContext dbContext, ILogger<EmployeeService> logger) : IEmployeeService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<EmployeeService> _logger = logger;

    // Example method to get the current employee
    public Task<Employee> GetCurrentEmployeeAsync()
    {
        // Implementation here
        throw new NotImplementedException();
    }
    // Example method to search employees by query
    public async Task<BaseResponse<List<Employee>>> SearchEmployeesAsync(string query)
    {
        var response = new BaseResponse<List<Employee>>();

        var  employees = await _dbContext.Employees.Where(t=>t.UserName.Contains(query) || 
                                   t.Email.Contains(query)).ToListAsync();
        response.Data = employees;
        response.Success = true;
        return response;

    }
    // Example method to get profile picture URL by email
    public async Task<(byte[] ImageBytes, string ContentType)> GetProfilePictureAsync(string email)
    {
        var employee = _dbContext.Employees.FirstOrDefault(e => e.Email == email);

        if (employee == null)
        {
            _logger.LogWarning("Employee with email {Email} not found.", email);
            return (null,null);
        }
        if (string.IsNullOrEmpty(employee.ProfilePictureUrl))
        {
            _logger.LogInformation("No profile picture found for employee with email {Email}.", email);
            return (null, null);
        }
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AppreciateApp/1.0)");
            var response = await httpClient.GetAsync(employee.ProfilePictureUrl);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to download profile picture for {Email}. Status: {StatusCode}, URL: {Url}, Response: {Response}", email, response.StatusCode, employee.ProfilePictureUrl, content);
                return (null, null);
            }
            var pictureBytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            return (pictureBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading profile picture for employee with email {Email}.", email);
            return (null,null);
        }
    }

    public Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        throw new NotImplementedException();
        // Upload the Profile Image to Azure Blob Storage and get the URL.
        // Save the employee details to the database along with the Profile Url.
        // Create a RabbitMQ message to notify other services.
    }
}