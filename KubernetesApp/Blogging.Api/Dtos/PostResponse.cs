namespace Blogging.Api.Dtos;

public class PostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string Category { get; set; }

}
