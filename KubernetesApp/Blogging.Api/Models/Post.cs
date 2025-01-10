namespace Blogging.Api.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? CategoryId { get; set; }
    public Category Category { get; set; }

}
