namespace OrderApiService.Models;

public enum OrderStatus
{
    Created=1,
    Pending=2,
    Processing=3,
    Shipped=4,
    Delivered=5,
    Cancelled=6
}
