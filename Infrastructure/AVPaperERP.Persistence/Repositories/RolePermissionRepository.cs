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
    public class RolePermissionRepository : GenericRepository, IRolePermissionRepository
    {
        private IConfiguration _configuration;

        public RolePermissionRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region Module Master 

        public async Task<IEnumerable<ModuleMaster_Response>> GetModuleMasterList(BaseSearchEntity parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<ModuleMaster_Response>("GetModuleMasterList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        #endregion

        #region Role Master Permission 

        public async Task<int> SaveRoleMasterPermission(RolePermission_Request parameters)
        {
            int result = 0;
            foreach (var item in parameters.ModuleList)
            {
                DynamicParameters queryParameters = new DynamicParameters();
                queryParameters.Add("@RolePermissionId", parameters.RolePermissionId);
                queryParameters.Add("@RoleId", parameters.RoleId);
                queryParameters.Add("@ModuleId", item.ModuleId);
                queryParameters.Add("@AppType", parameters.AppType);
                queryParameters.Add("@View", item.View);
                queryParameters.Add("@Add", item.Add);
                queryParameters.Add("@Edit", item.Edit);
                queryParameters.Add("@EmployeeId", 0);
                queryParameters.Add("@IsActive", parameters.IsActive);
                queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

                result = await SaveByStoredProcedure<int>("SaveRoleMasterPermission", queryParameters);
            }
            return result;
        }

        public async Task<IEnumerable<RolePermission_Response>> GetRoleMasterPermissionList(RolePermission_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@RoleId", parameters.RoleId);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<RolePermission_Response>("GetRoleMaster_PermissionList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<RolePermission_Response?> GetRoleMasterPermissionById(long Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<RolePermission_Response>("GetRegionById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region Role Master Employee Permission 

        public async Task<int> SaveRoleMasterEmployeePermission(RoleMaster_Employee_Permission_Request parameters)
        {
            int result = 0;

            if (parameters.ModuleList != null && parameters.ModuleList.Count > 0)
            {
                foreach (var item in parameters.ModuleList)
                {
                    DynamicParameters queryParameters = new DynamicParameters();
                    queryParameters.Add("@RolePermissionId", parameters.RolePermissionId);
                    queryParameters.Add("@RoleId", parameters.RoleId);
                    queryParameters.Add("@ModuleId", item.ModuleId);
                    queryParameters.Add("@AppType", parameters.AppType);
                    queryParameters.Add("@View", item.View);
                    queryParameters.Add("@Add", item.Add);
                    queryParameters.Add("@Edit", item.Edit);
                    queryParameters.Add("@EmployeeId", parameters.EmployeeId);
                    queryParameters.Add("@IsActive", parameters.IsActive);
                    queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

                    result = await SaveByStoredProcedure<int>("SaveRoleMasterPermission", queryParameters);
                }
            }
            else if (parameters.ModuleList == null)
            {
                DynamicParameters queryParameters = new DynamicParameters();
                queryParameters.Add("@RolePermissionId", parameters.RolePermissionId);
                queryParameters.Add("@RoleId", parameters.RoleId);
                queryParameters.Add("@ModuleId", 0);
                queryParameters.Add("@AppType", string.Empty);
                queryParameters.Add("@View", 0);
                queryParameters.Add("@Add", 0);
                queryParameters.Add("@Edit", 0);
                queryParameters.Add("@EmployeeId", parameters.EmployeeId);
                queryParameters.Add("@IsActive", 0);
                queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

                result = await SaveByStoredProcedure<int>("SaveRoleMasterPermission", queryParameters);
            }

            return result;
        }

        public async Task<IEnumerable<RoleMaster_Employee_Permission_Response>> GetRoleMasterEmployeePermissionList(RoleMaster_Employee_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@RoleId", 0);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<RoleMaster_Employee_Permission_Response>("GetRoleMaster_PermissionList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<IEnumerable<RoleMaster_Employee_Permission_Response>> GetRoleMasterEmployeePermissionById(long EmployeeId)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@RoleId", 0);
            queryParameters.Add("@EmployeeId", EmployeeId);
            queryParameters.Add("@IsActive", true);
            queryParameters.Add("@PageNo", 0);
            queryParameters.Add("@PageSize", 0);
            queryParameters.Add("@Total", 0, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", 0);

            var result = await ListByStoredProcedure<RoleMaster_Employee_Permission_Response>("GetRoleMaster_PermissionList", queryParameters);

            return result;
        }

        #endregion
    }
}
