using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantSystem.Infrastructure.Hubs;

[Authorize(Roles = "Admin,Manager")]
public class DashboardHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");
        await base.OnConnectedAsync();
    }
}
