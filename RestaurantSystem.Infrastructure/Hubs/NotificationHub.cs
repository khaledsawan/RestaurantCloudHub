using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantSystem.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
