using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace AVPaperERP.Persistence.Repositories
{
    public class ProfileRepository : GenericRepository, IProfileRepository
    {

        private IConfiguration _configuration;

        public ProfileRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region Department

        public async Task<int> SaveDepartment(Department_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@DepartmentName", parameters.DepartmentName.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveDepartment", queryParameters);
        }

        public async Task<IEnumerable<Department_Response>> GetDepartmentList(BaseSearchEntity parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Department_Response>("GetDepartmentList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<Department_Response?> GetDepartmentById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<Department_Response>("GetDepartmentById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region Role 

        public async Task<int> SaveRole(Role_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@RoleName", parameters.RoleName.SanitizeValue());
            queryParameters.Add("@DepartmentId", parameters.DepartmentId.SanitizeValue());
            queryParameters.Add("@EmployeeLevelId", parameters.EmployeeLevelId.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveRole", queryParameters);
        }

        public async Task<IEnumerable<Role_Response>> GetRoleList(BaseSearchEntity parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Role_Response>("GetRoleList", queryParameters);
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

        public async Task<Role_Response?> GetRoleById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<Role_Response>("GetRoleById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region RoleHierarchy 

        public async Task<int> SaveRoleHierarchy(RoleHierarchy_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@RoleId", parameters.RoleId);
            queryParameters.Add("@ReportingTo", parameters.ReportingTo);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveRoleHierarchy", queryParameters);
        }

        public async Task<IEnumerable<RoleHierarchy_Response>> GetRoleHierarchyList(BaseSearchEntity parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<RoleHierarchy_Response>("GetRoleHierarchyList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<RoleHierarchy_Response?> GetRoleHierarchyById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<RoleHierarchy_Response>("GetRoleHierarchyById", queryParameters)).FirstOrDefault();
        }

        #endregion

        //public async Task<IEnumerable<RoleResponse>> GetRolesList(SearchRoleRequest parameters)
        //{
        //    DynamicParameters queryParameters = new DynamicParameters();
        //    queryParameters.Add("@PageNo", parameters.pagination.PageNo);
        //    queryParameters.Add("@PageSize", parameters.pagination.PageSize);
        //    queryParameters.Add("@Total", parameters.pagination.Total, null, System.Data.ParameterDirection.Output);
        //    queryParameters.Add("@SortBy", parameters.pagination.SortBy.SanitizeValue());
        //    queryParameters.Add("@OrderBy", parameters.pagination.OrderBy.SanitizeValue());
        //    queryParameters.Add("@RoleName", parameters.RoleName.SanitizeValue());
        //    queryParameters.Add("@IsActive", parameters.IsActive);

        //    var result = await ListByStoredProcedure<RoleResponse>("GetRoles", queryParameters);
        //    parameters.pagination.Total = queryParameters.Get<int>("Total");

        //    return result;
        //}
    }
}
