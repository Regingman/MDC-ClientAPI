using System.Threading.Tasks;
using MyDataCoin.Entities;
using MyDataCoin.Models;

namespace MyDataCoin.Interfaces
{
    public interface IUser
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);

        Task<GeneralResponse> Upload(Uploadrequest model);

        Task<GeneralResponse> EditUser(string email, EditRequest user);

        RefreshResponse Refresh(Tokens tokens);

        Task<GeneralResponse> Mapping(string userid, MappingRequest model);

        Task<StatisticsOfRefferedPeopleModel> GetRefferedPeople(string userid);



        UserRefreshTokens AddUserRefreshTokens(UserRefreshTokens user);

        UserRefreshTokens GetSavedRefreshTokens(string socialId, string refreshtoken);

        void DeleteUserRefreshTokens(string socialId, string refreshToken);

        int SaveCommit();


        //Task<AuthenticateResponse> Registration(RegistrationRequest model);

        //Task<User> GetByEmail(string email);

        // Task<GeneralResponse> VerifyUser(string email, string code);

        // Task<GeneralResponse> SendCode(string email);

        // Task<GeneralResponse> ResetPassword(string email);
    }
}
