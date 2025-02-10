using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(ILogger<FilesController> logger,
                                 IWebHostEnvironment env) : ControllerBase
    {
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    logger.LogError("No file uploaded");
                    return BadRequest("No file uploaded");
                }

                var uploadPath = Path.Combine(env.ContentRootPath, "uploads");
                

                var filePath = Path.Combine(uploadPath, file.FileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                logger.LogInformation($"File {file.FileName} uploaded successfully");
                return Ok(new { filePath });

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
