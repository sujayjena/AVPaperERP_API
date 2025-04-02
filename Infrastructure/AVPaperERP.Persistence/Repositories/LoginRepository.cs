using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Persistence.Repositories
{
    public class LoginRepository : GenericRepository, ILoginRepository
    {
        private IConfiguration _configuration;

        public LoginRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<UsersLoginSessionData?> ValidateUserLoginByEmail(LoginByMobileNumberRequestModel parameters)
        {
            IEnumerable<UsersLoginSessionData> lstResponse;
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Username", parameters.MobileNumber.SanitizeValue());
            queryParameters.Add("@Password", parameters.Password.SanitizeValue());
            queryParameters.Add("@MobileUniqueId", parameters.MobileUniqueId.SanitizeValue());

            lstResponse = await ListByStoredProcedure<UsersLoginSessionData>("ValidateUserLoginByUsername", queryParameters);
            return lstResponse.FirstOrDefault();
        }

        public async Task SaveUserLoginHistory(UserLoginHistorySaveParameters parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@UserId", parameters.UserId);
            queryParameters.Add("@UserToken", parameters.UserToken.SanitizeValue());
            queryParameters.Add("@TokenExpireOn", parameters.TokenExpireOn);
            queryParameters.Add("@DeviceName", parameters.DeviceName.SanitizeValue());
            queryParameters.Add("@IPAddress", parameters.IPAddress.SanitizeValue());
            queryParameters.Add("@RememberMe", parameters.RememberMe);
            queryParameters.Add("@IsLoggedIn", parameters.IsLoggedIn);

            await ExecuteNonQuery("SaveUserLoginHistory", queryParameters);
        }

        public async Task<UsersLoginSessionData?> GetProfileDetailsByToken(string token)
        {
            IEnumerable<UsersLoginSessionData> lstResponse;
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Token", token);
            lstResponse = await ListByStoredProcedure<UsersLoginSessionData>("GetProfileDetailsByToken", queryParameters);

            return lstResponse.FirstOrDefault();
        }

        public async Task<int> ValidateUserMobile(OTPRequestModel parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@MobileNumber", parameters.MobileNumber.SanitizeValue());

            return await SaveByStoredProcedure<int>("ValidateUserMobile", queryParameters);
        }

        public async Task<int> SaveOTP(OTPRequestModel parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id",0);
            queryParameters.Add("@TemplateName", parameters.TemplateName.SanitizeValue());
            queryParameters.Add("@Mobile", parameters.MobileNumber.SanitizeValue());
            queryParameters.Add("@OTP", parameters.OTP.SanitizeValue());

            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveOTP", queryParameters);
        }

        public async Task<int> VerifyOTP(OTPVerifyModel parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Mobile", parameters.MobileNumber.SanitizeValue());
            queryParameters.Add("@OTP", parameters.OTP.SanitizeValue());

            return await SaveByStoredProcedure<int>("VerifyOTP", queryParameters);
        }
    }
}
