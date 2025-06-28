using AppreciateAppApi.DTO;
using AppreciateAppApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace AppreciateAppApi.Services;

public interface IEmployeeService
{
    Task<Employee> GetCurrentEmployeeAsync();
    Task<BaseResponse<List<Employee>>> SearchEmployeesAsync(string query);
    Task<(byte[] ImageBytes, string ContentType)> GetProfilePictureAsync(string email);
    Task<Employee> CreateEmployeeAsync(CreateEmployeeRequest request);
}
