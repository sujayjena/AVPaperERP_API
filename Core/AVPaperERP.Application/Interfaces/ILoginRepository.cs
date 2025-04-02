using AVPaperERP.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface ILoginRepository
    {
        Task<UsersLoginSessionData?> ValidateUserLoginByEmail(LoginByMobileNumberRequestModel parameters);

        Task SaveUserLoginHistory(UserLoginHistorySaveParameters parameters);

        Task<UsersLoginSessionData?> GetProfileDetailsByToken(string token);

        Task<int> ValidateUserMobile(OTPRequestModel parameters);

        Task<int> SaveOTP(OTPRequestModel parameters);

        Task<int> VerifyOTP(OTPVerifyModel parameters);
    }
}
