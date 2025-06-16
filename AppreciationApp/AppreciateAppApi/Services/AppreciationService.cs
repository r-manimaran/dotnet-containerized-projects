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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AppreciationService> _logger;

        public AppreciationService(AppDbContext dbContext, 
                                   IMapper mapper,
                                   IHttpContextAccessor httpContextAccessor,
                                   ILogger<AppreciationService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
            // Get the sender employee Id from the JWT token
            var claims = _httpContextAccessor.HttpContext?.User?.Claims;
            var email = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "email")?.Value;
            if(string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Sender email not found in the request.";
                return response;
            }
            // Look up the sender employee by email
            var senderEmployee = await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.Email == email);

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
            var claims = _httpContextAccessor.HttpContext?.User?.Claims;
            var emailClaim = claims?
                .FirstOrDefault(c =>
                    c.Type == "email" ||
                    c.Type == "emails" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            var email = emailClaim?.Value;
            if (!string.IsNullOrEmpty(email) && email.Contains(":"))
            {
                email = email.Split(':').Last().Trim();
            }
            //var email = _httpContextAccessor.HttpContext?.User?.Claims
            //   .FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Sender email not found in the request.";
                return response;
            }
            // Look up the sender employee by email
            var senderEmployee = await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.Email == email);

            // Get All Appreciation received by the logged in employee
            if (type == AppreciationType.Received && senderEmployee != null)
            {
                var query = _dbContext.Items.Where(i => i.Receivers.Any(r => r.Id == senderEmployee.Id));

                var count = await query.CountAsync();

                var result = await query
                                   .AsNoTracking()
                                   .OrderByDescending(a => a.CreatedAt)
                                   .Include(i => i.Category)
                                   .Include(i => i.Sender)
                                   .Include(i => i.Receivers)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

                PagedList<Item> pagedList = new PagedList<Item>(result, count, page, pageSize);

                var appreciationDto = _mapper.Map<List<AppreciationItem>>(pagedList, opt=> opt.Items["AppreciationType"]=type);
                
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
            else
            {
                response.Success = false;
                response.Message = "Logged In User Not found.";
                return response;
            }
        }

        public async Task<BaseResponse<Appreciation?>> GetAppreciationByIdAsync(int id)
        {
            var response = new BaseResponse<Appreciation?>();
            var appreciation  = await _dbContext.Items.AsNoTracking()
                                        .Include(i => i.Category)
                                        .Include(i => i.Sender)
                                        .Include(i => i.Receivers)
                                        .FirstOrDefaultAsync(i => i.Id == id);
            if (appreciation == null)
            {   
                response.Success = false;
                response.Message = "Appreciation not found.";
                return response;
            }
            var appreciationDto = _mapper.Map<Appreciation>(appreciation);
            response.Success = true;
            response.Data = appreciationDto;
            response.Message = "Appreciation retrieved successfully.";
            return response;
        }
    }
}
