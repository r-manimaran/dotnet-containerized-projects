using Blogging.Api.Dtos;
using Blogging.Api.Models;

namespace Blogging.Api.Services;

public interface IPostService 
{
    Task<IEnumerable<PostResponse>> GetAllAsync();
    Task<PostResponse> GetByIdAsync(Guid id);
    Task<PostResponse> CreateAsync(CreatePostRequest post);
    Task<PostResponse> UpdateAsync(Post post);
    Task DeleteAsync(Guid id);
}
