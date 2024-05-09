using Bislerium.Application.IServices;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.HubService
{
    public class FirebaseService : IFirebaseService
    {
        public FirebaseService(string serviceAccountKeyPath)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
            });
        }

        public async Task<string> SendNotificationAsync(string title, string body, string deviceToken)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Token = deviceToken
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return response;
            }
            catch (FirebaseMessagingException ex)
            {
                // Handle exceptions
                return ex.Message;
            }
        }
    }
}
