using AppreciateAppApi.Models;
using AppreciateAppApi.Pagination;

namespace AppreciateAppApi.DTO.Appreciation;

public class AppreciationResponse
{
    public List<AppreciationItem> Appreciations { get; set; } = new List<AppreciationItem>();
    public Metadata Metadata { get; set; } = new Metadata();
}
public class AppreciationItem
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public Category Category { get; set; }
    public Sender Sender { get; set; } = new Sender();
    public List<Receiver> Receivers { get; set; } = new List<Receiver>();
    public AppreciationType AppreciationType { get; set; }

}
