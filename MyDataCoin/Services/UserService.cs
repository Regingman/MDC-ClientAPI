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
        private readonly IJWTManager _jWTManager;

        private static string ApiKey = Environment.GetEnvironmentVariable("G_API_KEY");
        private static string Bucket = "mydatacoin.appspot.com";
        private static string AuthEmail = "img@gmail.com";
        private static string AuthPassword = Environment.GetEnvironmentVariable("G_API_PASSWORD");

        public UserService(WebApiDbContext db, IConfiguration conf, ILogger<UserService> logger, IJWTManager jWTManager)
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

                    if(user != null)
                        return new AuthenticateResponse(200, "Success", new AuthBody(user, token));
                    else
                    {
                        var newUser = StaticFunctions.CreateUser(model);
                        return new AuthenticateResponse(200, "Success", new AuthBody(newUser, token));
                    }
                }
                else
                {
                    var user = StaticFunctions.CreateUser(model);

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


        public async Task<GeneralResponse> Mapping(string userid, MappingRequest model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userid));

            if (model.SocialNetwork == "meta" && user.FacebookId == null)
                user.FacebookId = model.SocialId;

            else if (model.SocialNetwork == "meta" && user.FacebookId != null)
                return new GeneralResponse(436, "Already Mapped");

            else if (model.SocialNetwork == "google" && user.GoogleId == null)
                user.GoogleId = model.SocialId;

            else if (model.SocialNetwork == "google" && user.GoogleId != null)
                return new GeneralResponse(436, "Already Mapped");

            else if (model.SocialNetwork == "apple" && user.AppleId == null)
                user.AppleId = model.SocialId;

            else if(model.SocialNetwork == "apple" && user.AppleId != null)
                return new GeneralResponse(436, "Already Mapped");

            else
                return new GeneralResponse(400, "Wrong Social Network parameter");


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
                

                string add = $"?alt=media&token={Environment.GetEnvironmentVariable("G_IMAGE_TOKEN")}";

                string[] words = task.TargetUrl.Split("?");
                string[] words2 = words[1].Split("=");
                string finalString = words[0] + "/" + words2[1] + add;

                user.ProfilePic = finalString;

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


        public async Task<StatisticsOfRefferedPeopleModel> GetRefferedPeople(string userid)
        {
            Entities.User user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userid));
            int refferedUsers = await _db.Users.Where(x => x.CameFrom == user.RefCode).CountAsync();
            double refferedAmount = refferedUsers * 2.5;
            return new StatisticsOfRefferedPeopleModel(refferedUsers, refferedAmount);
        }

        public async Task<UserForMainPage> GetById(string userid)
        {
            Entities.User user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Guid.Parse(userid));
            UserForMainPage result = new UserForMainPage();
            result.NickName = user.NickName;
            result.ProfilePic = user.ProfilePic;
            return result;
        }

        public string GetPrivacy()
        {
            return @"<html><body>

                        <h4>MyDataCoin Privacy Policy</h4>
                        <p>MyDataCoin(“us”, “we”, or “our”) operates the http://www.mydatacoin.io website (the “Service”).</p>
                        <p>This page informs you of our policies regarding collecting, using, and disclosing personal data when you use our Service and the choices you have associated with that data.</p>
                        <p>We use your data to provide and improve the Service. By using the Service, you agree to collect and use information under this policy.Unless otherwise defined in this Privacy Policy, terms used in this Privacy Policy have the same meanings as in our Terms and Conditions, accessible from http://www.mydatacoin.io</p>

                        <h4>Your Data Protection Rights Under General Data Protection Regulation (GDPR)</h4>
                        <p>If you are a resident of the European Economic Area (EEA), you have certain data protection rights. myData.coin aims to take reasonable steps to allow you to correct, amend, delete, or limit the use of your Personal Data.</p>
                        <p>If you wish to be informed what Personal Data we hold about you and if you want it to be removed from our systems, please contact us.</p>
                        <p>In certain circumstances, you have the following data protection rights:</p>
                        <p>• The right to access, update or to delete the information we have on you. Whenever made possible, you can access, update or request deletion of your Personal Data directly within your account settings section. If you are unable to perform these actions yourself, please contact us to assist you.</p>
                        <p>• The right of rectification. You have the right to have your information rectified if that information is inaccurate or incomplete.</p>
                        <p>• The right to object. You have the right to object to our processing of your Personal Data.</p>
                        <p>• The right of restriction. You have the right to request that we restrict the processing of your personal information.</p>
                        <p>• The right to data portability. You have the right to be provided with a copy of the information we have on you in a structured, machine-readable and commonly used format.</p>
                        <p>• The right to withdraw consent. You also have the right to withdraw your consent at any time where myData.coin relied on your consent to process your personal information.</p>
                        <p>Please note that we may ask you to verify your identity before responding to such requests.</p>
                        <p>You have the right to complain to a Data Protection Authority about our collection and use of your Personal Data. For more information, please contact your local data protection authority in the European Economic Area (EEA).</p>

                        <h4>Service Providers</h4>
                        <p> We may employ third party companies and individuals to facilitate our Service(“Service Providers”), to provide the Service on our behalf, to perform Service - related services or to assist us in analyzing how our Service is used.
                                These third parties have access to your Personal Data only to perform these tasks on our behalf and are obligated not to disclose or use it for any other purpose.</p>

                        <h4>Analytics</h4>
                        <p>We may use third-party Service Providers to monitor and analyze the use of our Service.</p>
                        <p>• Google Analytics</p>
                        <p>• Google Analytics is a web analytics service offered by Google that tracks and reports website traffic. Google uses the data collected to track and monitor the use of our Service. This data is shared with other Google services. Google may use the collected data to contextualize and personalize the ads of its own advertising network.</p>
                        <p>• You can opt-out of having made your activity on the Service available to Google Analytics by installing the Google Analytics opt- out browser add-on. The add-on prevents the Google Analytics JavaScript (ga.js, analytics.js, and dc.js) from sharing information with Google Analytics about visits activity. For more information on the privacy practices of Google, please visit the Google Privacy & Terms web page: http://www.google.com/intl/en/policies/ privacy</p>

                        <h4>Children’s Privacy</h4>
                        <p>Our Service does not address anyone under the age of 18 (“Children”). We do not knowingly collect personally identifiable information from anyone under the age of 18. If you are a parent or guardian and you are aware that your Children has provided us with Personal Data, please contact us. If we become aware that we have collected Personal Data from children without verification of parental consent, we take steps to remove that information from our servers.</p>
                    
                        <h4>Changes To This Privacy Policy</h4>
                        <p>We may update our Privacy Policy from time to time. We will notify you of any changes by posting the new Privacy Policy on this page.</p>
                        <p>We will let you know via email and/or a prominent notice on our Service, prior to the change becoming effective and update the “effective date” at the top of this Privacy Policy.</p>
                        <p>You are advised to review this Privacy Policy periodically for any changes. Changes to this Privacy Policy are effective when they are posted on this page.</p>
                    

    </ body></html>";
        }

        public string GetTerms()
        {
            return "<html><body><h1>This is Privacy Policy page</h1></body></html>";
        }
    }
}
