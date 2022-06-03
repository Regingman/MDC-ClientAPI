﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyDataCoin.Entities;
using MyDataCoin.Interfaces;

namespace MyDataCoin.Services
{
	public class JWTManagerRepository : IJWTManagerRepository
	{
		public JWTManagerRepository()
		{
		}
		public Tokens GenerateToken(string socialId)
		{
			return GenerateJWTTokens(socialId);
		}

		public Tokens GenerateRefreshToken(string socialId)
		{
			return GenerateJWTTokens(socialId);
		}

		public Tokens GenerateJWTTokens(string socialId)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var tokenKey = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"));
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
				  {
				 new Claim(ClaimTypes.Name, socialId)
				  }),
					Expires = DateTime.Now.AddMinutes(10),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var refreshToken = GenerateRefreshToken();
				return new Tokens { Access_Token = tokenHandler.WriteToken(token), Refresh_Token = refreshToken };
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var Key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("DB_CONNECTION"));

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Key),
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}


			return principal;
		}

	}
}