namespace ProductsApi.Models;

public class Product
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ManufactoringDate { get; set; }
}
