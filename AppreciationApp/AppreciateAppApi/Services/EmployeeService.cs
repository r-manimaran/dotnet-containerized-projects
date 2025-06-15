using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace AppreciateAppApi.Services;

public class EmployeeService(AppDbContext dbContext, IBlobStorageService blogStorageService, ILogger<EmployeeService> logger) : IEmployeeService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IBlobStorageService _blogStorageService = blogStorageService;
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

    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        // Upload the Profile Image to Azure Blob Storage and get the URL.
        _logger.LogInformation($"Uploading profile image to Azure Blob Storage");
        var isUploaded = await _blogStorageService.UploadProfileAsync("profiles", request.ProfileImage.FileName, request.ProfileImage.OpenReadStream());
        if(!isUploaded)
        {
            _logger.LogError("Failed to upload profile image to Azure Blob Storage.");
            throw new Exception("Failed to upload profile image.");
        }
        _logger.LogInformation($"Profile image uploaded successfully.");
        string imageUrl = await _blogStorageService.GetBlob("profiles", request.ProfileImage.FileName);
        _logger.LogInformation($"Got the Image Url:{imageUrl}");
           
        // Save the employee details to the database along with the Profile Url.
        Employee employee = new Employee();
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;

        var userName = $"{request.LastName.ToLower().Substring(0, 1)}{request.FirstName.ToLower()}";
        employee.UserName = userName ;
        employee.Email = $"{userName}@example.com";

        employee.ProfilePictureUrl = imageUrl;
        employee.CreatedAt = DateTime.UtcNow;

        _logger.LogInformation($"Creating Employee: {employee.UserName}");
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Employee Created: {employee.UserName} with ID: {employee.Id}");
        return employee;
        // Create a RabbitMQ message to notify other services.
    }
}