using AppreciateAppApi.Data;
using AppreciateAppApi.Models;

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
    public Task<List<Employee>> SearchEmployeesAsync(string query)
    {
        // Implementation here
        throw new NotImplementedException();
    }
    // Example method to get profile picture URL by email
    public async Task<byte[]> GetProfilePictureAsync(string email)
    {
        // Implementation here
        var employee = _dbContext.Employees.FirstOrDefault(e => e.Email == email);

        if (employee == null)
        {
            _logger.LogWarning("Employee with email {Email} not found.", email);
            return null;
        }
        if (string.IsNullOrEmpty(employee.ProfilePictureUrl))
        {
            _logger.LogInformation("No profile picture found for employee with email {Email}.", email);
            return null;
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
                return null;
            }
            var pictureBytes = await response.Content.ReadAsByteArrayAsync();
           
            return pictureBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading profile picture for employee with email {Email}.", email);
            return null;
        }
    }
}