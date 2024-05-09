using Bislerium.Domain.Entity.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface INotificationService
    {
        Task<PushNotification> Create(PushNotification notification);
        Task<PushNotification> Read(Guid id);
        Task<List<PushNotification>> ListUsersNotification(string userId);
    }
}
