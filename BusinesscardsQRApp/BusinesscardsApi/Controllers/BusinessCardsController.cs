using BusinesscardsApi.Pagination;
using BusinesscardsApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BusinesscardsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardsController : ControllerBase
    {
        private readonly IBusinessCardService _businessCardService;

        public BusinessCardsController(IBusinessCardService businessCardService)
        {
            _businessCardService = businessCardService;
        }
        [HttpGet("bussinessusers")]
        public async Task<IActionResult> GetBusinessCards([FromQuery]RequestParameters requestParameter,CancellationToken token)
        {
            var pagedResult = await _businessCardService.GetBusinessCardUsers(requestParameter, token);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metadata));

            return Ok(pagedResult.businessCards);
        }
    }
}
