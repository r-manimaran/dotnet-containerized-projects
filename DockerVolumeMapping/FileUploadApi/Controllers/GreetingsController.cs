using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreetingsController(IConfiguration configuration) : ControllerBase
    {
        [HttpGet("message")]
        public IActionResult Get()
        {
            var message = configuration["GreetingMessage"];
            message = $"{message}-{DateTime.Now}";
            return Ok(message);
        }
    }
}
