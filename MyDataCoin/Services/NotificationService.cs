using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;

namespace MyDataCoin.Services
{
	public class NotificationService : INotification
	{
		private readonly IUser _userService;

		public NotificationService(IUser userService)
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

        public async Task<GeneralResponse> SendNotification(FCMMessage message)
        {
            try
            {
                var messageFCM = await CreateNotificationAsync(message);
                if (messageFCM == null)
                {
                    return new GeneralResponse(421, "User not found from this user id!");
                }
                var app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("data_coin.json")
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                });
                var messaging = FirebaseMessaging.GetMessaging(app);
                var result = await messaging.SendAsync(messageFCM);
                return new GeneralResponse(200, "Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(400, "There was an error sending your message, please try again later.");
            }
        }
    }
}

