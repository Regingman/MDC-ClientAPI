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
        private readonly FirebaseApp _firebaseApp;
        private readonly FirebaseMessaging _firebaseMessaging;
        public FiresBaseService(IUser userService)
        {
            _userService = userService;
            if (_firebaseApp != null)
            {
                var json = DotNetEnv.Env.GetString("FCM_CONFIGURATION", "Variable not found");
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(json)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                });
                if (_firebaseMessaging != null)
                {
                    _firebaseMessaging = FirebaseMessaging.GetMessaging(_firebaseApp);
                }
            }
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
                var result = await _firebaseMessaging.SendAsync(messageFCM);
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
