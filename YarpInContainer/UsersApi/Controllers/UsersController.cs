using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersApi.Data;
using UsersApi.Dtos;
using UsersApi.Models;

namespace UsersApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly UserDbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, UserDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserRequest request)
    {
        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            CreatedOn = DateTime.UtcNow
        };
        _dbContext.users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return Ok(newUser);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _dbContext.users.ToListAsync();

        return Ok(users);
    }
}
