using Firebase.Auth;
using Firebase.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyDataCoin.DataAccess;
using MyDataCoin.Entities;
using MyDataCoin.Helpers;
using MyDataCoin.Interfaces;
using MyDataCoin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyDataCoin.Services
{
    public class UserService : IUser
    {
        private readonly WebApiDbContext _db;
        private readonly IConfiguration _conf;
        private readonly ILogger<UserService> _logger;
        private readonly IJWTManagerRepository _jWTManager;

        private static string ApiKey = Environment.GetEnvironmentVariable("G_API_KEY");
        private static string Bucket = "mydatacoin.appspot.com";
        private static string AuthEmail = "img@gmail.com";
        private static string AuthPassword = Environment.GetEnvironmentVariable("G_API_PASSWORD");

        public UserService(WebApiDbContext db, IConfiguration conf, ILogger<UserService> logger, IJWTManagerRepository jWTManager)
        {
            _db = db;
            _conf = conf;
            _logger = logger;
            _jWTManager = jWTManager;
        }


        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            try
            {
                var existingUser = await _db.Emails.SingleOrDefaultAsync(x => x.EmailAddress == model.Email);

                if (existingUser != null)
                {
                    var token = _jWTManager.GenerateToken(model.SocialId);

                    if (token == null)
                    {
                        return new AuthenticateResponse(401,"Invalid Attempt!");
                    }

                    // saving refresh token to the db
                    UserRefreshTokens obj = new UserRefreshTokens
                    {
                        RefreshToken = token.Refresh_Token,
                        SocialId = model.SocialId
                    };

                    AddUserRefreshTokens(obj);
                    SaveCommit();

                    var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == existingUser.UserId);
                    return new AuthenticateResponse(200, "Success", new AuthBody(user, token));
                }
                else
                {
                    var user = new Entities.User();

                    string walletAddress = $"mdc{StaticFunctions.GeneratePromoCode(17)}";

                    if (model.SocialNetwork == "meta") user.FacebookId = model.SocialId;
                    else if (model.SocialNetwork == "google") user.GoogleId = model.SocialId;
                    else if (model.SocialNetwork == "apple") user.AppleId = model.SocialId;
                    else return new AuthenticateResponse(400, "Wrong Social Network parameter");

                    user.Id = Guid.NewGuid();
                    user.NickName = model.NickName;
                    user.CreatedAt = DateTime.UtcNow;
                    user.DeviceId = model.DeviceId;
                    user.Balance = 0;
                    user.WalletAddress = walletAddress;
                    user.RefCode = StaticFunctions.GeneratePromoCode(8);
                    user.FCMToken = model.FCMToken;

                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    Email email = new Email()
                    {
                        EmailAddress = model.Email,
                        UserId = user.Id,
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        SocialNetworkName = model.SocialNetwork
                    };

                    await _db.Emails.AddAsync(email);
                    await _db.SaveChangesAsync();

                    var token = _jWTManager.GenerateToken(model.SocialId);

                    if (token == null)
                    {
                        return new AuthenticateResponse(401, "Invalid Attempt");
                    }

                    // saving refresh token to the db
                    UserRefreshTokens obj = new UserRefreshTokens
                    {
                        RefreshToken = token.Refresh_Token,
                        SocialId = model.SocialId
                    };

                    AddUserRefreshTokens(obj);
                    SaveCommit();

                    return new AuthenticateResponse(200, "Success", new AuthBody(user, token));
                }
            }
            catch(Exception ex)
            {
                return new AuthenticateResponse(400, ex.InnerException.Message);
            }
        }


        public RefreshResponse Refresh(Tokens tokens)
        {
            var principal = _jWTManager.GetPrincipalFromExpiredToken(tokens.Access_Token);
            var socialId = principal.Identity?.Name;

            //retrieve the saved refresh token from database
            var savedRefreshToken = GetSavedRefreshTokens(socialId, tokens.Refresh_Token);

            if (savedRefreshToken.RefreshToken != tokens.Refresh_Token)
            {
                return new RefreshResponse(401, "Invalid attempt!");
            }

            var newJwtToken = _jWTManager.GenerateRefreshToken(socialId);

            if (newJwtToken == null)
            {
                return new RefreshResponse(401, "Invalid attempt!");
            }

            // saving refresh token to the db
            UserRefreshTokens obj = new UserRefreshTokens
            {
                RefreshToken = newJwtToken.Refresh_Token,
                SocialId = socialId,
            };

            DeleteUserRefreshTokens(socialId, tokens.Refresh_Token);
            AddUserRefreshTokens(obj);
            SaveCommit();

            return new RefreshResponse(200, newJwtToken);
        }


        public async Task<GeneralResponse> Mapping(string userid, AuthenticateRequest model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userid));

            if (model.SocialNetwork == "meta" && user.FacebookId == null) user.FacebookId = model.SocialId;
            else if (model.SocialNetwork == "google" && user.GoogleId == null) user.GoogleId = model.SocialId;
            else if (model.SocialNetwork == "apple" && user.AppleId == null) user.AppleId = model.SocialId;
            else return new GeneralResponse(400, "Wrong Social Network parameter");


            Email email = new Email()
            {
                EmailAddress = model.Email,
                UserId = Guid.Parse(userid),
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                SocialNetworkName = model.SocialNetwork
            };

            await _db.Emails.AddAsync(email);
            await _db.SaveChangesAsync();

            //return new AuthenticateResponse(200, "Success", new AuthBody(user, null));
            return new GeneralResponse(200, "Success");
        }



        public async Task<GeneralResponse> Upload(Uploadrequest model)
        {
            byte[] bytes = Convert.FromBase64String(model.ImageData);
            MemoryStream stream = new MemoryStream(bytes);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("Images")
                .Child($"{model.UserId}.{model.Extension}")
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            try
            {
                // error during upload will be thrown when you await the task
                //Console.WriteLine("Download link:\n" + await task);
                Entities.User user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(model.UserId));
                user.ProfilePic = task.TargetUrl;

                string add = $"?alt=media&token={Environment.GetEnvironmentVariable("G_IMAGE_TOKEN")}";

                string[] words = task.TargetUrl.Split('?');
                string[] words2 = words[1].Split('=');
                string finalString = words[0] + "/" + words2[1] + add;

                await _db.SaveChangesAsync();
                return new GeneralResponse(200, finalString);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
                //Console.WriteLine("Exception was thrown: {0}", ex);
            }
        }

        public UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user)
        {
            _db.UserRefreshToken.Add(user);
            return user;
        }

        public void DeleteUserRefreshTokens(string socialId, string refreshToken)
        {
            UserRefreshTokens item = _db.UserRefreshToken.FirstOrDefault(x => x.SocialId == socialId && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _db.UserRefreshToken.Remove(item);
            }
        }

        public UserRefreshTokens GetSavedRefreshTokens(string socialId, string refreshToken)
        {
            return _db.UserRefreshToken.FirstOrDefault(x => x.SocialId == socialId && x.RefreshToken == refreshToken && x.IsActive == true);
        }

        public int SaveCommit()
        {
            return _db.SaveChanges();
        }


        public async Task<GeneralResponse> EditUser(string id, EditRequest user)
        {
            Entities.User existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(id));
            if (user == null) return new GeneralResponse(421, "User Not Found");
            else
            {
                if (user.NickName != null && user.NickName != existingUser.NickName)
                    existingUser.NickName = user.NickName;

                await _db.SaveChangesAsync();

                return new GeneralResponse(204, "No Content");
            }
        }


        
        public async Task<StatisticsOfRefferedPeopleModel> GetRefferedPeople(string userid)
        {
            Entities.User user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userid));
            int refferedUsers = await _db.Users.Where(x => x.CameFrom == user.RefCode).CountAsync();
            double refferedAmount = refferedUsers * 2.5;
            return new StatisticsOfRefferedPeopleModel(refferedUsers, refferedAmount);
        }

        public async Task<string> GetTokenFromUserId(string userId)
        {
            Entities.User user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userId));
            return user.FCMToken;
        }

        public async Task<List<Entities.User>> GetAllUsersId()
        {
            var users = await _db.Users.ToListAsync();
            return users;
        }
    }
}
