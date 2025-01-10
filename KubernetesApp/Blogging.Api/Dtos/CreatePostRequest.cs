namespace Blogging.Api.Dtos;

public class CreatePostRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int CategoryId { get; set; }
}
