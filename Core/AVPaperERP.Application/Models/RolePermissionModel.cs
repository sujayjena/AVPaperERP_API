using AVPaperERP.Domain.Entities;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class RolePermissionModel
    {
    }

    #region ModuleMaster

    public class ModuleMaster_Request //: BaseEntity
    {
        public long ModuleId { get; set; }
        public long ModuleName { get; set; }
        public long AppType { get; set; }
        public long IsActive { get; set; }
    }

    public class ModuleMaster_Response //: BaseResponseEntity
    {
        public long ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? AppType { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ModuleList
    {
        public long ModuleId { get; set; }
        public bool? View { get; set; }
        public bool? Add { get; set; }
        public bool? Edit { get; set; }
    }

    #endregion

    #region RolePermission

    public class RolePermission_Search : BaseSearchEntity
    {
        public long RoleId { get; set; }

        public long? EmployeeId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class RolePermission_Request // : BaseEntity
    {
        [Required]
        public long RolePermissionId { get; set; }

        public long? RoleId { get; set; }

        public string? AppType { get; set; }

        public bool? IsActive { get; set; }

        public List<ModuleList> ModuleList { get; set; }
    }

    public class RolePermission_Response
    {
        public string? AppType { get; set; }

        //public long RolePermissionId { get; set; }
        //public long? RoleId { get; set; }
        //public string? RoleName { get; set; }

        public long? ModuleId { get; set; }

        public string? ModuleName { get; set; }

        public int? View { get; set; }

        public int? Add { get; set; }

        public int? Edit { get; set; }

        public bool? IsActive { get; set; }
    }

    #endregion

    #region RoleMaster_Employee_Permission

    public class RoleMaster_Employee_Search_Request : BaseSearchEntity
    {
        public long EmployeeId { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    public class RoleMaster_Employee_Permission_Request // : BaseEntity
    {
        public long RolePermissionId { get; set; }
        public long RoleId { get; set; }
        public string? AppType { get; set; }
        public long EmployeeId { get; set; }
        public bool? IsActive { get; set; }

        public List<ModuleList>? ModuleList { get; set; }
    }

    public class RoleMaster_Employee_Permission_Response 
    {
        public string? AppType { get; set; }
        //public long EmployeePermissionId { get; set; }
        //public long EmployeeId { get; set; }
        //public long RoleId { get; set; }
        //public string? RoleName { get; set; }
        public long ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public bool? View { get; set; }
        public bool? Add { get; set; }
        public bool? Edit { get; set; }
    }

    #endregion
}
