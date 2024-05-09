using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Bislerium.WebApp.HubService
{
    public class NotificationHub : Hub
    {
        //public async Task SendNotification(string receiverId, string message)
        //{
        //    string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var users = new string[] { senderId, receiverId };

        //    await Clients.Users(users).SendAsync("ReceiveNotification", senderId, receiverId, message);
        //}
    }
}
