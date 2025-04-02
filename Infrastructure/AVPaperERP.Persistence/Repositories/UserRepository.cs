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
    public class UserRepository : GenericRepository, IUserRepository
    {
        private IConfiguration _configuration;

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region User 

        public async Task<int> SaveUser(User_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@UserCode", parameters.UserCode);
            queryParameters.Add("@UserName", parameters.UserName);
            queryParameters.Add("@MobileNumber", parameters.MobileNumber);
            queryParameters.Add("@EmailId", parameters.EmailId);
            queryParameters.Add("@Password", !string.IsNullOrWhiteSpace(parameters.Password) ? EncryptDecryptHelper.EncryptString(parameters.Password) : string.Empty);
            queryParameters.Add("@UserType", parameters.UserType);
            queryParameters.Add("@RoleId", parameters.RoleId);
            queryParameters.Add("@ReportingTo", parameters.ReportingTo);
            queryParameters.Add("@CompanyId", parameters.CompanyId);
            //queryParameters.Add("@BranchId", parameters.BranchId);
            queryParameters.Add("@DepartmentId", parameters.DepartmentId);
            queryParameters.Add("@AddressLine", parameters.AddressLine);
            queryParameters.Add("@StateId", parameters.StateId);
            queryParameters.Add("@DistrictId", parameters.DistrictId);
            queryParameters.Add("@CityId", parameters.CityId);
            queryParameters.Add("@Pincode", parameters.Pincode);
            queryParameters.Add("@DateOfBirth", parameters.DateOfBirth);
            queryParameters.Add("@DateOfJoining", parameters.DateOfJoining);
            queryParameters.Add("@EmergencyContactNumber", parameters.EmergencyContactNumber);
            queryParameters.Add("@BloodGroupId", parameters.BloodGroupId);
            queryParameters.Add("@MobileUniqueId", parameters.MobileUniqueId);
            queryParameters.Add("@AadharNumber", parameters.AadharNumber);
            queryParameters.Add("@AadharOriginalFileName", parameters.AadharOriginalFileName);
            queryParameters.Add("@AadharImage", parameters.AadharImage);
            queryParameters.Add("@PanNumber", parameters.PanNumber);
            queryParameters.Add("@PanCardOriginalFileName", parameters.PanCardOriginalFileName);
            queryParameters.Add("@PanCardImage", parameters.PanCardImage);
            queryParameters.Add("@ProfileOriginalFileName", parameters.ProfileOriginalFileName);
            queryParameters.Add("@ProfileImage", parameters.ProfileImage);
            queryParameters.Add("@IsMobileUser", parameters.IsMobileUser);
            queryParameters.Add("@IsWebUser", parameters.IsWebUser);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveUser", queryParameters);
        }

        public async Task<IEnumerable<User_Response>> GetUserList(BaseSearchEntity parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<User_Response>("GetUserList", queryParameters);

            if (SessionManager.LoggedInUserId > 1)
            {
                if (SessionManager.LoggedInUserId > 2)
                {
                    result = result.Where(x => x.Id > 2).ToList();
                }
                else
                {
                    result = result.Where(x => x.Id > 1).ToList();
                }
            }

            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<User_Response?> GetUserById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<User_Response>("GetUserById", queryParameters)).FirstOrDefault();
        }

        public async Task<IEnumerable<UserListByRole_Response>> GetUserLisByRoleIdOrRoleName(UserListByRole_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@RoleId", parameters.RoleId.SanitizeValue());
            queryParameters.Add("@RoleName", parameters.RoleName.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);

            var result = await ListByStoredProcedure<UserListByRole_Response>("GetUserByRoleIdOrRoleName", queryParameters);

            return result;
        }

        public async Task<IEnumerable<User_ImportDataValidation>> ImportUser(List<User_ImportData> parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            string xmlData = ConvertListToXml(parameters);
            queryParameters.Add("@XmlData", xmlData);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await ListByStoredProcedure<User_ImportDataValidation>("ImportUser", queryParameters);
        }

        #endregion
    }
}
