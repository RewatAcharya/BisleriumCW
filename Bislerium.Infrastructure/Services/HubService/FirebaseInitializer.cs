using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.HubService
{
    public class FirebaseInitializer
    {
        public static void InitializeFirebaseAdminSDK(string serviceAccountKeyPath)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
            });
        }
    }
}
