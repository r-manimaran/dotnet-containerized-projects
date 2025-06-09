using AppreciateAppApi.Data;
using AppreciateAppApi.DTO;
using AppreciateAppApi.DTO.Appreciation;
using AppreciateAppApi.Models;
using AppreciateAppApi.Pagination;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppreciateAppApi.Services
{
    public class AppreciationService : IAppreciationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<AppreciationService> _logger;

        public AppreciationService(AppDbContext dbContext, 
                                   IMapper mapper,
                                   ILogger<AppreciationService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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
            };
            // Add receivers to the appreciation
            appreciation.Receivers = receiverEmployees;
            
            // save to database
            _dbContext.Items.Add(appreciation);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Appreciation created successfully. Id: {appreciation.Id}");

            Appreciation newAppreciation = new Appreciation
            {

                Content = appreciation.Content,
                
                // Category = category,
                // Sender = $"{senderEmployee.FirstName} {senderEmployee.LastName}",
                // Receivers = receiverEmployees.Select(r => $"{r.FirstName} {r.LastName}").ToList(),
                // AppreciationType = appreciation.AppreciationType.ToString(),
                // Id = appreciation.Id
            };

            response.Success = true;
            response.Data = newAppreciation;
            response.Message = "Appreciation created successfully.";
            return response;
        }

        public async Task<BaseResponse<AppreciationResponse>> GetAllAppreciation(int page, int pageSize, AppreciationType type)
        {
            BaseResponse<AppreciationResponse> response = new ();

            AppreciationResponse appreciationResponse = new ();

            // Perform the paginated query
            // Need to update this based on User Claims
            var query = _dbContext.Items.Where(i => i.SenderId == 1);
            
            var count = await query.CountAsync();

            var result = await query
                               .OrderByDescending(a=>a.CreatedAt)
                               .Include(i => i.Category)                               
                               .Skip((page - 1) * pageSize)                            
                               .Take(pageSize)
                               .ToListAsync();

            PagedList<Item> pagedList = new PagedList<Item>(result, count, page, pageSize);

            var appreciationDto = _mapper.Map<List<AppreciationItem>>(pagedList);

            appreciationResponse.Appreciations = appreciationDto;
            
            appreciationResponse.Metadata = pagedList.Metadata;

            if (pagedList == null || !pagedList.Any())
            {
                response.Success = false;
                response.Message = "No appreciations found.";
                return response;
            }

            response.Success = true;
            response.Message = "Appreciations retrieved successfully.";
            response.Data = appreciationResponse;            
                       
            return response;
        }
    }
}
