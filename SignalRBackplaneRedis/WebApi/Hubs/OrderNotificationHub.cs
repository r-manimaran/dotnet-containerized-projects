using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Models;

namespace WebApi.Hubs;

/// <summary>
/// Strongly typed client
/// </summary>
public interface IOrderNotificationClient
{
    Task OrderStatusUpdated(Order order, string instanceId);
}

[Authorize]
public class OrderNotificationHub : Hub<IOrderNotificationClient>
{

}
