using Bislerium.Domain.Entity.Notifications;
using Bislerium.Domain.Entity.Users;
using Bislerium.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.HubService
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationHub(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task SendPrivateMessage(string receiverId, string message)
        {
            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var users = new string[] { senderId, receiverId };

            var notification = new PushNotification
            {
                Sender = senderId,
                Receiver = receiverId,
                Message = message,
            };

            _db.PushNotifications.Add(notification);
            await _db.SaveChangesAsync();

            await Clients.Users(users).SendAsync("ReceivePrivateMessage", senderId, receiverId, message);
        }
    }
}
