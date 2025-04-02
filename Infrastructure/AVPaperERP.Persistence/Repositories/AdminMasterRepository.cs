using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace AVPaperERP.Persistence.Repositories
{
    public class AdminMasterRepository : GenericRepository, IAdminMasterRepository
    {

        private IConfiguration _configuration;

        public AdminMasterRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region Blood Group

        public async Task<int> SaveBloodGroup(BloodGroup_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@BloodGroup", parameters.BloodGroup);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveBloodGroup", queryParameters);
        }

        public async Task<IEnumerable<BloodGroup_Response>> GetBloodGroupList(BloodGroup_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<BloodGroup_Response>("GetBloodGroupList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<BloodGroup_Response?> GetBloodGroupById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<BloodGroup_Response>("GetBloodGroupById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region Company Type

        public async Task<int> SaveCompanyType(CompanyType_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@CompanyType", parameters.CompanyType.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveCompanyType", queryParameters);
        }

        public async Task<IEnumerable<CompanyType_Response>> GetCompanyTypeList(CompanyType_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<CompanyType_Response>("GetCompanyTypeList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<CompanyType_Response?> GetCompanyTypeById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<CompanyType_Response>("GetCompanyTypeById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region Employee Level

        public async Task<int> SaveEmployeeLevel(EmployeeLevel_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@EmployeeLevel", parameters.EmployeeLevel);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveEmployeeLevel", queryParameters);
        }

        public async Task<IEnumerable<EmployeeLevel_Response>> GetEmployeeLevelList(EmployeeLevel_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<EmployeeLevel_Response>("GetEmployeeLevelList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<EmployeeLevel_Response?> GetEmployeeLevelById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<EmployeeLevel_Response>("GetEmployeeLevelById", queryParameters)).FirstOrDefault();
        }

        #endregion
    }
}
