using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IFirebaseService
    {
        Task SendNotificationAsync(string userId, string title, string body);
        Task<FirebaseUserToken> CreateNewToken(CreateToken payload);
    }
}
