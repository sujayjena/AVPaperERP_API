using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface IAdminMasterRepository
    {
        #region Blood Group

        Task<int> SaveBloodGroup(BloodGroup_Request parameters);
        Task<IEnumerable<BloodGroup_Response>> GetBloodGroupList(BloodGroup_Search parameters);
        Task<BloodGroup_Response?> GetBloodGroupById(long Id);

        #endregion

        #region Company Type

        Task<int> SaveCompanyType(CompanyType_Request parameters);
        Task<IEnumerable<CompanyType_Response>> GetCompanyTypeList(CompanyType_Search parameters);
        Task<CompanyType_Response?> GetCompanyTypeById(long Id);

        #endregion

        #region Employee Level

        Task<int> SaveEmployeeLevel(EmployeeLevel_Request parameters);
        Task<IEnumerable<EmployeeLevel_Response>> GetEmployeeLevelList(EmployeeLevel_Search parameters);
        Task<EmployeeLevel_Response?> GetEmployeeLevelById(long Id);

        #endregion
    }
}
