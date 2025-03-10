using ECommerceApi.DTOs;
using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;

    public CustomerService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<Customer>> CreateCustomer(CustomerRequest customerRequest, 
                                        CancellationToken cancellationToken = default)
    {
        var newCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = customerRequest.Name,
            Email = customerRequest.Email
        };
        _dbContext.Customers.Add(newCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new ApiResponse<Customer>
        {
            Success = true,
            Message = "Customer Created Successfully",
            Data = newCustomer
        };
        return response;
    }

    public async Task<ApiResponse<Customer>> GetCustomerById(Guid Id, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers.AsNoTracking().Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
        if(customer is null)
        {
            throw new ApplicationException(string.Format("Customer with ID:{Id} does not exists", Id));           
        }

        var response = new ApiResponse<Customer>
        {
            Success = true,    
            Data = customer
        };

        return response;

    }

    public async Task<ApiResponse<IEnumerable<Customer>>> GetCustomers(CancellationToken cancellationToken = default)
    {
        var customers = await _dbContext.Customers.AsNoTracking().ToListAsync(cancellationToken);
        return new ApiResponse<IEnumerable<Customer>>
        {
            Success = true,
            Data = customers
        };
    }
}
