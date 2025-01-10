using Blogging.Api.Dtos;
using Blogging.Api.Models;
using Blogging.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogging.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _service;
        private readonly ILogger<PostsController> _logger;
        private readonly IValidator<CreatePostRequest> _validator;
        public PostsController(IPostService service, ILogger<PostsController> logger, IValidator<CreatePostRequest> validator)
        {
            _logger = logger;
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _service.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await _service.GetByIdAsync(id);
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            var validtionResult = await _validator.ValidateAsync(post);
            if (!validtionResult.IsValid)
            {
                throw new ValidationException(validtionResult.Errors);
            }

            var response = await _service.CreateAsync(post);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            var response = await _service.UpdateAsync(post);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

    }
}
