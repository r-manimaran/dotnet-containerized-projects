using AutoMapper;
using Blogging.Api.Data;
using Blogging.Api.Dtos;
using Blogging.Api.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Blogging.Api.Services;

public class PostService : IPostService
{
    private readonly BlogDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<PostService> _logger;
    private readonly IValidator<Post> _postValidator;
    private readonly IValidator<CreatePostRequest> _createPostRequestValidator;

    public PostService(BlogDbContext dbContext,
                       IMapper mapper,
                       ILogger<PostService> logger,
                       IValidator<Post> validator,
                       IValidator<CreatePostRequest> createPostRequestValidator)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _postValidator = validator;
        _createPostRequestValidator = createPostRequestValidator;
    }

    public async Task<PostResponse> CreateAsync(CreatePostRequest post)
    {
        if (post == null)
        {
            _logger.LogError("Post is null");
            throw new ArgumentNullException(nameof(post));
        }
        var validationResult = _createPostRequestValidator.Validate(post);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            throw new ValidationException(string.Join(", ", errors));
        }
        var newPost = _mapper.Map<Post>(post);  
        await _dbContext.Posts.AddAsync(newPost);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Post created");

        // Map the post to PostReponse using Automapper
        var response = _mapper.Map<PostResponse>(newPost);

        return response;

    }

    public Task DeleteAsync(Guid id)
    {
        var post = _dbContext.Posts.FirstOrDefault(p => p.Id == id);
        if (post is null)
        {
            _logger.LogError("Post not found");
            throw new Exception("Post not found");
        }
        _dbContext.Posts.Remove(post);
        _dbContext.SaveChanges();
        _logger.LogInformation("Post deleted");
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<PostResponse>> GetAllAsync()
    {
        var posts = await _dbContext.Posts.AsNoTracking()
                                         .Include(x => x.Category)
                                         .ToListAsync();

        // Map the response to the PostResponse using Automapper
        var response = _mapper.Map<IEnumerable<PostResponse>>(posts);

        return response;
    }

    public async Task<PostResponse> GetByIdAsync(Guid id)
    {
        var post = await _dbContext.Posts
                        .Include(c=>c.Category)
                        .FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            _logger.LogError("Post not found");
            throw new Exception("Post not found");
        }
        var response = _mapper.Map<PostResponse>(post);
        return response;
    }

    public async Task<PostResponse> UpdateAsync(Post post)
    {

        if (post == null)
        {
            _logger.LogError("Post is null");
            throw new ArgumentNullException(nameof(post));
        }

        var validationResult = _postValidator.Validate(post);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            throw new ValidationException(string.Join(", ", errors));
        }

        //update the existing post
        var existingPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);
        if (existingPost is null)
        {
            _logger.LogError("Post not found");
            throw new Exception("Post not found");
        }
        // use Automapper to map the post to the existing post
        _mapper.Map(post, existingPost);

        _dbContext.Posts.Update(existingPost);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Post updated");

        var response = _mapper.Map<PostResponse>(existingPost);
        return response;
    }
}
