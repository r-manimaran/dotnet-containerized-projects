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
            // Validation
            if (!validationResult.IsValid)
            {
                response.Success = false;
                var errors = validationResult.Errors.Select(e=>e.ErrorMessage).ToList();
                response.Message = string.Join(";", errors);
                return response;
            }

            // Check for email already exists
            var existingUser = await dbContext.Customers.FirstOrDefaultAsync(x=>x.Email == request.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Email already exists.";
                response.Data = existingUser;
                return response;
            }

            // Add and save the new customer
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

        public async Task<bool> DeleteCustomer(int customerId)
        {
            var existingCustomer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (existingCustomer == null)
            {
                return false;
            }
            dbContext.Customers.Remove(existingCustomer);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceResponse<List<Customer>>> GetAllCustomers()
        {
            var response = new ServiceResponse<List<Customer>>();
            var allCustomers = await dbContext.Customers.ToListAsync();
            response.Success = allCustomers.Any();
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
