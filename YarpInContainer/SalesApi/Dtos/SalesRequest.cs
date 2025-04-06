namespace SalesApi.Dtos;

public class SalesRequest
{
    public int UserId { get; set; }
    public List<int> Products { get; set; } = new();
}
