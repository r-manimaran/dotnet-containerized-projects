using AppreciateAppApi.Models;

namespace AppreciateAppApi.Services;

public interface IEmployeeService
{
    Task<Employee> GetCurrentEmployeeAsync();
    Task<List<Employee>> SearchEmployeesAsync(string query);
    Task<(byte[] ImageBytes, string ContentType)> GetProfilePictureAsync(string email);
}
