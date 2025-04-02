using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;

namespace AVPaperERP.Application.Interfaces
{
    public interface IProfileRepository
    {
        #region Department

        Task<int> SaveDepartment(Department_Request parameters);

        Task<IEnumerable<Department_Response>> GetDepartmentList(BaseSearchEntity parameters);

        Task<Department_Response?> GetDepartmentById(long Id);

        #endregion

        #region Role 

        Task<int> SaveRole(Role_Request parameters);

        Task<IEnumerable<Role_Response>> GetRoleList(BaseSearchEntity parameters);

        Task<Role_Response?> GetRoleById(long Id);

        #endregion

        #region RoleHierarchy 

        Task<int> SaveRoleHierarchy(RoleHierarchy_Request parameters);

        Task<IEnumerable<RoleHierarchy_Response>> GetRoleHierarchyList(BaseSearchEntity parameters);

        Task<RoleHierarchy_Response?> GetRoleHierarchyById(long Id);

        #endregion
    }
}
