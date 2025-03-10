using ECommerceApi.DTOs;
using ECommerceApi.Models;

namespace ECommerceApi.Services;

public interface ICustomerService
{
    Task<ApiResponse<Customer>> CreateCustomer(CustomerRequest customerRequest, CancellationToken cancellationToken=default);
    Task<ApiResponse<Customer>> GetCustomerById(Guid Id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<Customer>>> GetCustomers(CancellationToken cancellationToken = default);
}
