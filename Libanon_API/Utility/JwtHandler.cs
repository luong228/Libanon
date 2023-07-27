using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.IdentityModel.Tokens;

namespace Libanon_API.Utility
{
    public class JwtHandler : IJwtHandler
    {
        public string EncodeJwtToken(Claim[] claims)
        {

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["secret_key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "anhluong059",
                audience: "libanon",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            // Generate Jwt string
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
            //End Jwt
        }

        public IDictionary<string, string> DecodeJwtToken(string jwtToken)
        {
            //Decode JWT
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["secret_key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            try
            {
                var claimsPrinciple = handler.ValidateToken(jwtToken, validationParameters, out var securityToken);
                var claims = claimsPrinciple.Claims;
                return claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch (SecurityTokenException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}