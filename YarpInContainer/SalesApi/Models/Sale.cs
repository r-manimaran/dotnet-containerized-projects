namespace SalesApi.Models;

public class Sale
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<int> Products { get; set; }
    public DateTime CreatedOn { get; set; }
}
