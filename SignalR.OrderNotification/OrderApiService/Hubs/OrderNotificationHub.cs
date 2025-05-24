using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OrderApiService.Models;

namespace OrderApiService.Hubs;
public interface IOrderNotificationClient
{
    Task OrderStatusUpdated(Order order);
}

[Authorize]
public sealed class OrderNotificationHub : Hub<IOrderNotificationClient>
{
    //public async Task SendOrderStatusUpdate(Order order)
    //{
    //    await Clients.All.SendAsync("OrderStatusUpdated", order);
    //}
}
