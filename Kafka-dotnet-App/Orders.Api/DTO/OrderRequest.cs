namespace Orders.Api.DTO
{
    public record OrderRequest(int CustomerId, string ProductName, int quantity, decimal Price);
    
}
