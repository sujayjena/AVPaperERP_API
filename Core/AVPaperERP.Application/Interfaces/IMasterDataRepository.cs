using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface IMasterDataRepository
    {
        Task<IEnumerable<SelectListResponse>> GetReportingToEmployeeForSelectList(ReportingToEmpListParameters parameters);
        Task<IEnumerable<EmployeesListByReportingTo_Response>> GetEmployeesListByReportingTo(int EmployeeId);
    }
}
