using API.Utility;
using IdentityApp.Models;
using IdentityApp.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _jwtKey;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _jwtKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        }
        public string CreateJWT(AppUser user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(StaticDetails.UserId, user.Id.ToString()),
                new Claim(StaticDetails.Name, user.Name),
                new Claim(StaticDetails.UserName, user.UserName),
                new Claim(StaticDetails.Email, user.Email)
            };
            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"])),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
