using Azure;
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Services
{
    public class CustomerService(IValidator<CreateCustomerRequest> validator, AppDbContext dbContext) : ICustomerService
    {
        public async Task<ServiceResponse<Customer>> CreateCustomer(CreateCustomerRequest request)
        {
            var response = new ServiceResponse<Customer>();
            var validationResult = validator.Validate(request);
            if (validationResult.IsValid)
            { 
                
            }

            var newCustomer = new Customer()
            {
                Email = request.Email,
                Name = request.Name,
            };

            dbContext.Customers.Add(newCustomer);
            await dbContext.SaveChangesAsync();
            response.Success = true;
            response.Data = newCustomer;
            return response;
        }

        public Task<bool> DeleteCustomer(int customerId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<Customer>>> GetAllCustomers()
        {
            var response = new ServiceResponse<List<Customer>>();
            var allCustomers = await dbContext.Customers.ToListAsync();
            response.Success &= allCustomers.Any();
            response.Data = allCustomers;
            return response;
        }

        public async Task<ServiceResponse<Customer>> GetCustomerById(int customerId)
        {
            var response = new ServiceResponse<Customer>();
            var customer = await dbContext.Customers.SingleOrDefaultAsync(i=>i.Id == customerId);
            if (customer == null)
            {
                response.Success = false;
                response.Message = $"Customer with Id {customerId} not found.";
                return response;
            }
            response.Data = customer;
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<Customer>> UpdateCustomer(UpdateCustomerRequest request)
        {
            var response = new ServiceResponse<Customer>();
            var existingCustomer = await dbContext.Customers.SingleOrDefaultAsync( i => i.Id == request.Id);
            if (existingCustomer == null)
                {
                response.Success = false;
                response.Message = $"Customer with Id {request.Id} not found.";
                return response;
            }
            existingCustomer.Name = request.Name;
            existingCustomer.Email = request.Email;
            await dbContext.SaveChangesAsync();
            response.Success = true;
            response.Data = existingCustomer;
            return response;
        }
    }
}
