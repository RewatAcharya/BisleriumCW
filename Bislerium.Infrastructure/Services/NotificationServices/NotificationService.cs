using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Notifications;
using Bislerium.Infrastructure.Data;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {

        private readonly ApplicationDbContext _dbContext;

        public NotificationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PushNotification> Create(PushNotification notification)
        {
            var result = await _dbContext.PushNotifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<PushNotification>> ListUsersNotification(string userId)
        {
            var result = await _dbContext.PushNotifications.Where(x => x.Receiver == userId).ToListAsync();
            return result.ToList();
        }

        public async Task<PushNotification> Read(Guid id)
        {
            var result = await _dbContext.PushNotifications.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return null;
            }
            
            result.IsRead = true;
            _dbContext.PushNotifications.Update(result);
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
