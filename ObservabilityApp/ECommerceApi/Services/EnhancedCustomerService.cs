using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Tracing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerceApi.Services;

public class EnhancedCustomerService : ICustomerService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EnhancedCustomerService> _logger;
    private readonly HttpClient _httpClient;

    public EnhancedCustomerService(AppDbContext dbContext, ILogger<EnhancedCustomerService> logger, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<Customer>> CreateCustomer(CustomerRequest customerRequest, CancellationToken cancellationToken = default)
    {
        using var activity = TracingService.StartActivity("CreateCustomer.BusinessLogic");
        activity?.SetTag("customer.email", customerRequest.Email);
        activity?.SetTag("customer.name", customerRequest.Name);

        // Email validation span
        using var validationActivity = TracingService.StartActivity("ValidateCustomerEmail");
        validationActivity?.SetTag("validation.type", "email");
        await ValidateEmailAsync(customerRequest.Email);

        // Database operation span
        using var dbActivity = TracingService.StartActivity("SaveCustomerToDatabase");
        var newCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = customerRequest.Name,
            Email = customerRequest.Email
        };

        dbActivity?.SetTag("customer.id", newCustomer.Id.ToString());
        _dbContext.Customers.Add(newCustomer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // External service call span
        using var notificationActivity = TracingService.StartActivity("SendWelcomeNotification", ActivityKind.Client);
        await SendWelcomeNotificationAsync(newCustomer);

        activity?.SetTag("operation.result", "success");
        activity?.SetTag("customer.created_id", newCustomer.Id.ToString());

        return new ApiResponse<Customer>
        {
            Success = true,
            Message = "Customer Created Successfully",
            Data = newCustomer
        };
    }

    public async Task<ApiResponse<Customer>> GetCustomerById(Guid id, CancellationToken cancellationToken = default)
    {
        using var activity = TracingService.StartActivity("GetCustomerById.BusinessLogic");
        activity?.SetTag("customer.lookup_id", id.ToString());

        // Cache check span
        using var cacheActivity = TracingService.StartActivity("CheckCustomerCache");
        var cachedCustomer = await CheckCacheAsync(id);
        if (cachedCustomer != null)
        {
            activity?.SetTag("cache.hit", true);
            return new ApiResponse<Customer> { Success = true, Data = cachedCustomer };
        }
        activity?.SetTag("cache.hit", false);

        // Database lookup span
        using var dbActivity = TracingService.StartActivity("DatabaseCustomerLookup");
        var customer = await _dbContext.Customers.AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (customer is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Customer not found");
            activity?.SetTag("error.type", "NotFound");
            throw new ApplicationException($"Customer with ID:{id} does not exist");
        }

        activity?.SetTag("operation.result", "success");
        return new ApiResponse<Customer> { Success = true, Data = customer };
    }

    public async Task<ApiResponse<IEnumerable<Customer>>> GetCustomers(CancellationToken cancellationToken = default)
    {
        using var activity = TracingService.StartActivity("GetAllCustomers.BusinessLogic");
        
        // Performance monitoring span
        using var perfActivity = TracingService.StartActivity("MeasureQueryPerformance");
        var stopwatch = Stopwatch.StartNew();
        
        var customers = await _dbContext.Customers.AsNoTracking().ToListAsync(cancellationToken);
        
        stopwatch.Stop();
        perfActivity?.SetTag("query.duration_ms", stopwatch.ElapsedMilliseconds);
        perfActivity?.SetTag("query.result_count", customers.Count);
        
        if (stopwatch.ElapsedMilliseconds > 1000)
        {
            perfActivity?.SetTag("performance.slow_query", true);
            _logger.LogWarning("Slow query detected: {Duration}ms", stopwatch.ElapsedMilliseconds);
        }

        activity?.SetTag("customers.count", customers.Count);
        return new ApiResponse<IEnumerable<Customer>> { Success = true, Data = customers };
    }

    private async Task ValidateEmailAsync(string email)
    {
        using var activity = TracingService.StartActivity("EmailValidation");
        activity?.SetTag("email.domain", email.Split('@').LastOrDefault());
        
        // Simulate email validation logic
        await Task.Delay(50);
        
        if (!email.Contains("@"))
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Invalid email format");
            throw new ArgumentException("Invalid email format");
        }
        
        activity?.SetTag("validation.result", "valid");
    }

    private async Task<Customer?> CheckCacheAsync(Guid id)
    {
        using var activity = TracingService.StartActivity("CacheOperation");
        activity?.SetTag("cache.key", $"customer:{id}");
        
        // Simulate cache check
        await Task.Delay(10);
        
        activity?.SetTag("cache.provider", "redis");
        return null; // No cache implementation for demo
    }

    private async Task SendWelcomeNotificationAsync(Customer customer)
    {
        using var activity = TracingService.StartActivity("ExternalNotificationService", ActivityKind.Client);
        activity?.SetTag("notification.type", "welcome_email");
        activity?.SetTag("notification.recipient", customer.Email);
        
        try
        {
            // Simulate external API call
            await Task.Delay(200);
            activity?.SetTag("notification.status", "sent");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            throw;
        }
    }
}