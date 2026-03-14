using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantSystem.Infrastructure.Hubs;

[Authorize(Roles = "Delivery,Admin,Manager")]
public class DeliveryHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Delivery");
        await base.OnConnectedAsync();
    }
}
