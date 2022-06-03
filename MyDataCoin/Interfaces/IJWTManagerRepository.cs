using System.Security.Claims;
using MyDataCoin.Entities;

namespace MyDataCoin.Interfaces
{
    public interface IJWTManagerRepository
    {
        Tokens GenerateToken(string socialId);

        Tokens GenerateRefreshToken(string socialId);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
