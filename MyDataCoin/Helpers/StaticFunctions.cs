using System;
using System.Linq;
using MyDataCoin.Entities;
using MyDataCoin.Models;

namespace MyDataCoin.Helpers
{
	public static class StaticFunctions
	{
        public static User CreateUser(AuthenticateRequest model)
        {
            User user = new User();
            string walletAddress = $"mdc{GeneratePromoCode(17)}";

            if (model.SocialNetwork == "meta") user.FacebookId = model.SocialId;
            else if (model.SocialNetwork == "google") user.GoogleId = model.SocialId;
            else if (model.SocialNetwork == "apple") user.AppleId = model.SocialId;
            else return new User();

            user.Id = Guid.NewGuid();
            user.NickName = model.NickName;
            user.CreatedAt = DateTime.UtcNow;
            user.DeviceId = model.DeviceId;
            user.Balance = 0;
            user.WalletAddress = walletAddress;
            user.RefCode = GeneratePromoCode(8);
            user.FCMToken = model.FCMToken;

            return user;
        }

        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static string GeneratePromoCode(int length)
        {
            Random random = new Random();
            const string chars = "0123456789ABCDEFGHIJKLMNOPRSTUVWXYZ";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

