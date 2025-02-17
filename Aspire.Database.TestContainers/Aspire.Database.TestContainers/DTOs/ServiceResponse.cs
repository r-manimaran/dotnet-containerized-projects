namespace Aspire.Database.TestContainers.DTOs;

public class ServiceResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = null;
    public T Data { get; set; } 

}
