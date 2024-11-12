using Microsoft.AspNetCore.SignalR;

namespace YourCare_Application.ServerHubs
{
    public class ServerHub: Hub
    {
        public async Task NotifyLogout(string userId)
        {
            await Clients.User(userId).SendAsync("ForceLogout");
        }
    }
}
