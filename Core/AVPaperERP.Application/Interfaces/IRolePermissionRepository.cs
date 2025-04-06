using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface IRolePermissionRepository
    {
        #region Module Master 

        Task<IEnumerable<ModuleMaster_Response>> GetModuleMasterList(BaseSearchEntity parameters);

        #endregion

        #region Role Master Permission 

        Task<int> SaveRoleMasterPermission(RolePermission_Request parameters);

        Task<IEnumerable<RolePermission_Response>> GetRoleMasterPermissionList(RolePermission_Search parameters);

        Task<RolePermission_Response?> GetRoleMasterPermissionById(long Id);

        #endregion

        #region Role Master Employee Permission 

        Task<int> SaveRoleMasterEmployeePermission(RoleMaster_Employee_Permission_Request parameters);

        Task<IEnumerable<RoleMaster_Employee_Permission_Response>> GetRoleMasterEmployeePermissionList(RoleMaster_Employee_Search_Request parameters);

        Task<IEnumerable<RoleMaster_Employee_Permission_Response>> GetRoleMasterEmployeePermissionById(long EmployeeId);

        #endregion
    }
}
