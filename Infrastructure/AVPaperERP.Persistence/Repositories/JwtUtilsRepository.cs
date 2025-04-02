using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Persistence.Repositories
{
    public class JwtUtilsRepository : IJwtUtilsRepository
    {
        private readonly AppSettings _appSettings;
        private readonly ILoginRepository _loginRepository;

        public JwtUtilsRepository(IOptions<AppSettings> appSettings, ILoginRepository loginRepository)
        {
            _appSettings = appSettings.Value;
            _loginRepository = loginRepository;
        }

        public (string, DateTime) GenerateJwtToken(UsersLoginSessionData parameters)
        {
            string token;
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.JWT.SecretKey);
            List<Claim> claims = new List<Claim>();
            DateTime tokenExpiryDateTime;

            claims.Add(new Claim("UserId", EncryptDecryptHelper.EncryptString(parameters.UserId.ToString())));
            claims.Add(new Claim("MobileNumber", parameters.MobileNumber));
            claims.Add(new Claim("EmailId", parameters.EmailId));

            if (parameters.UserId != null)
            {
                claims.Add(new Claim("UserCode", parameters.UserCode));
                claims.Add(new Claim("UserName", parameters.UserName));
                claims.Add(new Claim("RoleName", parameters.RoleName));
            }
            //else if (parameters.CustomerId != null)
            //{
            //    claims.Add(new Claim("Name", parameters.CompanyName));
            //    claims.Add(new Claim("CustomerTypeName", parameters.CustomerTypeName));
            //}

            //tokenExpiryDateTime = DateTime.Now.AddMinutes(60);
            tokenExpiryDateTime = DateTime.Now.AddDays(365);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiryDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            token = tokenHandler.WriteToken(securityToken);

            return (token, tokenExpiryDateTime);
        }

        public async Task<UsersLoginSessionData?> ValidateJwtToken(string token)
        {
            byte[] key;
            JwtSecurityTokenHandler tokenHandler;
            JwtSecurityToken jwtToken;
            UsersLoginSessionData? response;

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            try
            {
                tokenHandler = new JwtSecurityTokenHandler();
                key = Encoding.ASCII.GetBytes(_appSettings.JWT.SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                jwtToken = (JwtSecurityToken)validatedToken;

                response = await _loginRepository.GetProfileDetailsByToken(token);

                //Update LastAccessOn in UsersLoginHistory table


                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}