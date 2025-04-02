using AVPaperERP.API.CustomAttributes;
using AVPaperERP.Application.Constants;
using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Helpers;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace AVPaperERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ResponseModel _response;
        private ILoginRepository _loginRepository;
        private IJwtUtilsRepository _jwt;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly INotificationRepository _notificationRepository;
        private ISMSHelper _smsHelper;
        private readonly IConfigRefRepository _configRefRepository;

        public LoginController(ILoginRepository loginRepository, IJwtUtilsRepository jwt, IRolePermissionRepository rolePermissionRepository, IUserRepository userRepository, ICompanyRepository companyRepository, INotificationRepository notificationRepository, ISMSHelper smsHelper, IConfigRefRepository configRefRepository)
        {
            _loginRepository = loginRepository;
            _jwt = jwt;
            _userRepository = userRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _companyRepository = companyRepository;
            _notificationRepository = notificationRepository;
            _smsHelper = smsHelper;
            _configRefRepository = configRefRepository;

            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        /*
        [HttpPost]
        [Route("[action]")]
        public async Task<ResponseModel> OTPGenerate(OTPRequestModel parameters)
        {
            int result = await _loginRepository.ValidateUserMobile(parameters);

            if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "No record exists";
            }
            else
            {
                int iOTP = Utilities.GenerateRandomNumForOTP();
                if (iOTP > 0)
                {
                    parameters.OTP = Convert.ToString(iOTP);
                }

                // Opt save
                int resultOTP = await _loginRepository.SaveOTP(parameters);

                if (resultOTP > 0)
                {
                    _response.Message = "OTP sent successfully.";

                    //#region SMS Send

                    //var vConfigRef_Search = new ConfigRef_Search()
                    //{
                    //    Ref_Type = "SMS",
                    //    Ref_Param = "TicketGeneration"
                    //};

                    //string sSMSTemplateName = string.Empty;
                    //string sSMSTemplateContent = string.Empty;
                    //var vConfigRefObj = _configRefRepository.GetConfigRefList(vConfigRef_Search).Result.ToList().FirstOrDefault();
                    //if (vConfigRefObj != null)
                    //{
                    //    sSMSTemplateName = vConfigRefObj.Ref_Value1;
                    //    sSMSTemplateContent = vConfigRefObj.Ref_Value2;

                    //    if (!string.IsNullOrWhiteSpace(sSMSTemplateContent))
                    //    {
                    //        //Replace parameter 
                    //        sSMSTemplateContent = sSMSTemplateContent.Replace("{#var#}", parameters.MobileNumber);
                    //    }
                    //}

                    //var vsmsRequest = new SMS_Request()
                    //{
                    //    OTPId = resultOTP,
                    //    TemplateName = sSMSTemplateName,
                    //    TemplateContent = sSMSTemplateContent,
                    //    Mobile = parameters.MobileNumber,
                    //};

                    //bool bSMSResult = await _smsHelper.SMSSend(vsmsRequest);

                    //#endregion
                }
            }

            return _response;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ResponseModel> OTPVerification(OTPVerifyModel parameters)
        {
            //if (parameters.OTP == "1234")
            //{
            //    _response.Message = "OTP verified sucessfully.";
            //}
            //else
            //{
            int result = await _loginRepository.VerifyOTP(parameters);

            if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Invalid OTP!";
                _response.IsSuccess = false;
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "OTP timeout!";
                _response.IsSuccess = false;
            }
            else
            {
                _response.Message = "OTP verified sucessfully.";
            }
            //}

            return _response;
        }
        */

        [HttpPost]
        [Route("[action]")]
        public async Task<ResponseModel> MobileAppLogin(MobileAppLoginRequestModel parameters)
        {
            LoginByMobileNumberRequestModel loginParameters = new LoginByMobileNumberRequestModel();
            loginParameters.MobileNumber = parameters.MobileNumber;
            loginParameters.Password = parameters.Password;
            loginParameters.MobileUniqueId = parameters.MobileUniqueId;
            loginParameters.Remember = parameters.Remember;
            loginParameters.IsWebOrMobileUser = parameters.IsWebOrMobileUser;

            //_response.Data = await Login(loginParameters);

            var vLoginObj = await Login(loginParameters);

            return vLoginObj;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ResponseModel> Login(LoginByMobileNumberRequestModel parameters)
        {
            (string, DateTime) tokenResponse;
            SessionDataEmployee employeeSessionData;
            UsersLoginSessionData? loginResponse;
            UserLoginHistorySaveParameters loginHistoryParameters;

            parameters.Password = EncryptDecryptHelper.EncryptString(parameters.Password);

            loginResponse = await _loginRepository.ValidateUserLoginByEmail(parameters);

            if (loginResponse != null)
            {
                if (loginResponse.IsActive == true && (loginResponse.IsWebUser == true && parameters.IsWebOrMobileUser == "W" || loginResponse.IsMobileUser == true && parameters.IsWebOrMobileUser == "M"))
                {
                    tokenResponse = _jwt.GenerateJwtToken(loginResponse);

                    if (loginResponse.UserId != null)
                    {
                        string strBrnachIdList = string.Empty;

                        var vRoleList = await _rolePermissionRepository.GetRoleMasterEmployeePermissionById(Convert.ToInt64(loginResponse.UserId));

                        // Notification List
                        var vNotification_SearchObj = new Notification_Search()
                        {
                            NotifyDate = null,
                            UserId = Convert.ToInt32(loginResponse.UserId)
                        };

                        var vUserNotificationList = await _notificationRepository.GetNotificationList(vNotification_SearchObj);

                        var vUserDetail = await _userRepository.GetUserById(Convert.ToInt32(loginResponse.UserId));

                        employeeSessionData = new SessionDataEmployee
                        {
                            UserId = loginResponse.UserId,
                            UserCode = loginResponse.UserCode,
                            UserName = loginResponse.UserName,
                            MobileNumber = loginResponse.MobileNumber,
                            EmailId = loginResponse.EmailId,
                            UserType = loginResponse.UserType,
                            RoleId = loginResponse.RoleId,
                            RoleName = loginResponse.RoleName,
                            IsMobileUser = loginResponse.IsMobileUser,
                            IsWebUser = loginResponse.IsWebUser,
                            IsActive = loginResponse.IsActive,
                            Token = tokenResponse.Item1,

                            CompanyId = vUserDetail != null ? Convert.ToInt32(vUserDetail.CompanyId) : 0,
                            CompanyName = vUserDetail != null ? vUserDetail.CompanyName : String.Empty,
                            DepartmentId = vUserDetail != null ? Convert.ToInt32(vUserDetail.DepartmentId) : 0,
                            DepartmentName = vUserDetail != null ? vUserDetail.DepartmentName : String.Empty,
                            EmployeeLevelId = vUserDetail != null ? Convert.ToInt32(vUserDetail.EmployeeLevelId) : 0,
                            EmployeeLevel = vUserDetail != null ? vUserDetail.EmployeeLevel : String.Empty,

                            ProfileImage = vUserDetail != null ? vUserDetail.ProfileImage : String.Empty,
                            ProfileOriginalFileName = vUserDetail != null ? vUserDetail.ProfileOriginalFileName : String.Empty,
                            ProfileImageURL = vUserDetail != null ? vUserDetail.ProfileImageURL : String.Empty,

                            UserRoleList = vRoleList.ToList(),
                            UserNotificationList = vUserNotificationList.ToList()
                        };

                        _response.Data = employeeSessionData;
                    }

                    //Login History
                    loginHistoryParameters = new UserLoginHistorySaveParameters
                    {
                        UserId = loginResponse.UserId,
                        UserToken = tokenResponse.Item1,
                        IsLoggedIn = true,
                        IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        DeviceName = HttpContext.Request.Headers["User-Agent"],
                        TokenExpireOn = tokenResponse.Item2,
                        RememberMe = parameters.Remember
                    };

                    await _loginRepository.SaveUserLoginHistory(loginHistoryParameters);

                    _response.Message = MessageConstants.LoginSuccessful;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = ErrorConstants.InactiveProfileError;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid credential, please try again with correct credential";
            }

            return _response;
        }

        [HttpPost]
        [Route("[action]")]
        [CustomAuthorize]
        public async Task<ResponseModel> Logout()
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().SanitizeValue()!;
            //UsersLoginSessionData? sessionData = (UsersLoginSessionData?)HttpContext.Items["SessionData"]!;

            UserLoginHistorySaveParameters logoutParameters = new UserLoginHistorySaveParameters
            {
                UserId = SessionManager.LoggedInUserId,
                UserToken = token,
                IsLoggedIn = false, //To Logout set IsLoggedIn = false
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceName = HttpContext.Request.Headers["User-Agent"],
                TokenExpireOn = DateTime.Now,
                RememberMe = false
            };

            await _loginRepository.SaveUserLoginHistory(logoutParameters);

            _response.Message = MessageConstants.LogoutSuccessful;

            return _response;
        }
    }
}
