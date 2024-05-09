using Bislerium.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.HubService
{
    public class NotificationService1
    {
        private readonly IFirebaseService _firebaseService;

        public NotificationService1(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task SendNotification(string title, string body, string deviceToken)
        {
            await _firebaseService.SendNotificationAsync(title, body, deviceToken);
        }
    }
}
