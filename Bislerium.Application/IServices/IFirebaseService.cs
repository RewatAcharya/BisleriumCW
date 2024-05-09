using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IFirebaseService
    {
        Task<string> SendNotificationAsync(string title, string body, string deviceToken);
    }
}
