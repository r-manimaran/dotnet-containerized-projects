namespace ProductsApi.DTOs;

public class ProductRequest
{    
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
