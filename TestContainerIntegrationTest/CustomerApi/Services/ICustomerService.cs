using CustomerApi.DTOs;
using CustomerApi.Models;

namespace CustomerApi.Services
{
    public interface ICustomerService
    {
        Task<ServiceResponse<Customer>> CreateCustomer(CreateCustomerRequest request);
        Task<ServiceResponse<Customer>> UpdateCustomer(UpdateCustomerRequest request);
        Task<bool> DeleteCustomer(int customerId);
        Task<ServiceResponse<Customer>> GetCustomerById(int customerId);
        Task<ServiceResponse<List<Customer>>> GetAllCustomers();
    }
}
