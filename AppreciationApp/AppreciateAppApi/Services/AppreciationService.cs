using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;
using AppreciateAppApi.Pagination;
using Microsoft.EntityFrameworkCore;

namespace AppreciateAppApi.Services
{
    public class AppreciationService : IAppreciationService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AppreciationService> _logger;

        public AppreciationService(AppDbContext dbContext, 
                                   ILogger<AppreciationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        /// <summary>
        /// Implementation for creating new Appreciation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse<Appreciation?>> CreateAppreciationAsync(CreateAppreciationRequest request)
        {
            var response = new BaseResponse<Appreciation?>();

            // Validate sender
            var senderEmployee = await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.Id == request.SenderId);
            if(senderEmployee == null)
            {
                response.Success = false;
                response.Message = "Sender not found.";
                return response;
            }

            // Validate receivers
            if (request.Receivers == null || !request.Receivers.Any())
            {
                response.Success = false;
                response.Message = "At least one receiver is required";
                return response;
            }

            var receiverIds = request.Receivers.Select(r => r.EmployeeId)
                                               .ToList();

            var receiverEmployees = await _dbContext.Employees
                            .Where(e=>receiverIds.Contains(e.Id))
                            .ToListAsync();

            if (receiverEmployees.Count != receiverIds.Count)
            {
                response.Success = false;
                response.Message = "One or more receivers not found.";
                return response;
            }

            // validate category
            var category = await _dbContext.Categories
                                .FirstOrDefaultAsync(c=>c.Id == request.CategoryId);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return response;
            }

            // Create Appreciation entity
            var appreciation = new Item
            {
                CategoryId = request.CategoryId,
                Content = request.Content,
                SenderId = senderEmployee.Id,
                CreatedAt = DateTime.UtcNow,                
                Sender = new Sender
                {
                    EmployeeId = senderEmployee.Id,
                    UserName = senderEmployee.UserName,
                    FullName = $"{senderEmployee.FirstName} {senderEmployee.LastName}",
                    Email = senderEmployee.Email
                },
                Receivers = request.Receivers.Select(r => new Receiver
                {
                    EmployeeId = r.EmployeeId,
                    UserName = r.UserName,
                    FullName = r.FullName,
                    Email = r.Email
                }).ToList(),
               
            };

            // save to database
            _dbContext.Items.Add(appreciation);
            await _dbContext.SaveChangesAsync();

            response.Success = true;
            response.Data = appreciation;
            response.Message = "Appreciation created successfully.";
            return response;
        }

        public async Task<BaseResponse<AppreciationResponse>> GetAllAppreciation(int page, int pageSize, AppreciationType type)
        {
            BaseResponse<AppreciationResponse> response = new BaseResponse<AppreciationResponse>();
            var result = await _dbContext.Items
                        .OrderByDescending(a => a.CreatedAt)
                        .ToListAsync();
            response.Success = true;
            //response.Data = new AppreciationResponse
            //{
            //    Items = result.Select(a => new AppreciationItem
            //    {
            //        Id = a.Id,
            //        Content = a.Content,
            //        Category = a.Category,
            //        Sender = a.Sender,
            //        Receivers = a.Receivers.ToList(),
            //        AppreciationType = a.AppreciationType,
            //    }).ToList(),
            //    Metadata = null
            //};
            return response;
        }
    }
}
