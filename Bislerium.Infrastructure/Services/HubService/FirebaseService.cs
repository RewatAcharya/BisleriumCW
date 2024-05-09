using Bislerium.Application.IServices;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bislerium.Infrastructure.Data;
using Bislerium.Domain.ViewModels;
using System.Text.RegularExpressions;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Bislerium.Domain.Entity.Users;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.Infrastructure.Services.HubService
{
    public class FirebaseService : IFirebaseService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly object _lock = new object();
        private static bool _firebaseInitialized = false;

        public FirebaseService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
            InitializeFirebaseApp();
        }

        private void InitializeFirebaseApp()
        {
            lock (_lock)
            {
                if (!_firebaseInitialized && FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("C:\\Users\\zackz\\Desktop\\AD Group CW\\Bislerium\\Bislerium.Infrastructure\\App_Data\\fire425837619-firebase-adminsdk-ktmdc-09f72270ac.json")
                    });

                    _firebaseInitialized = true;
                }
            }
        }

        // Save Firebase Token
        public async Task<FirebaseUserToken> CreateNewToken(CreateToken payload)
        {
            var user = await _userManager.FindByIdAsync(payload.UserId);
            if (user == null)
                throw new Exception("Token cannot be saved before login.");

            // Check if the user already has a token
            var existingToken = await _dbContext.FirebaseUserTokens.FirstOrDefaultAsync(t => t.UserID == payload.UserId);

            if (existingToken != null)
            {
                // If the token already exists for the user, update it
                existingToken.Token = payload.Token;
                //await _dbContext.SaveChangesAsync();
                return existingToken;
            }
            else
            {
                // If the token doesn't exist for the user, create a new one
                var newToken = new FirebaseUserToken
                {
                    Token = payload.Token,
                    UserID = payload.UserId
                };
                await _dbContext.FirebaseUserTokens.AddAsync(newToken);
                await _dbContext.SaveChangesAsync();
                return newToken;
            }
        }

        // Send push notification
        public async Task SendNotificationAsync(string userId, string title, string body)
        {
            // Fetch tokens for the given user IDs
            var tokens = await _dbContext.FirebaseUserTokens
                .Where(token => userId.Contains(token.UserID) && token.Token != null)
                .Select(token => token.Token)
                .ToListAsync();

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }
}

