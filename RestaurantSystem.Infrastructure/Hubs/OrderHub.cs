using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantSystem.Infrastructure.Hubs;

[Authorize]
public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("Manager") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");
        }

        await base.OnConnectedAsync();
    }
}
