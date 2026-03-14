using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantSystem.Infrastructure.Hubs;

[Authorize(Roles = "Chef,Kitchen,Admin,Manager")]
public class KitchenHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Kitchen");
        await base.OnConnectedAsync();
    }
}
