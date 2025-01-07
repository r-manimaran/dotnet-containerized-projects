namespace InventoryApi.DTOs;

public record CreateProductRequest(string Name, decimal Price, int Quantity);


