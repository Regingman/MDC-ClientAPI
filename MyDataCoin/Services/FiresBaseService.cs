using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyDataCoin.Services
{
    public class FiresBaseService : IFireBase
    {
        private readonly IUser _userService;
        public FiresBaseService(IUser userService)
        {
            _userService = userService;
        }

        private async Task<Message> CreateNotificationAsync(FCMMessage message)
        {
            var token = await _userService.GetTokenFromUserId(message.UserId);
            return new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = message.Body,
                    Title = message.Title
                }
            };
        }

        public async Task<AuthenticateResponse> SendNotification(FCMMessage message)
        {
            try
            {
                var messageFCM = await CreateNotificationAsync(message);
                if (messageFCM == null)
                {
                    return new AuthenticateResponse(421, "User not found from this user id!");
                }
                var app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("data_coin.json")
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                });
                var messaging = FirebaseMessaging.GetMessaging(app);
                var result = await messaging.SendAsync(messageFCM);
                return new AuthenticateResponse(200, "Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new AuthenticateResponse(400, "There was an error sending your message, please try again later.");
            }
        }
    }
}
