using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class SelectListResponse
    {
        public long Value { get; set; }
        public string? Text { get; set; }
    }
    public class ReportingToEmpListParameters
    {
        public long RoleId { get; set; }
        public long? RegionId { get; set; }

        [DefaultValue(null)]
        public bool? IsActive { get; set; }
    }
    public partial class EmployeesListByReportingTo_Response
    {
        public int? Id { get; set; }
        public string? EmployeeName { get; set; }
        public int? CompanyId { get; set; }
        public string? BranchId { get; set; }
        public int? UserId { get; set; }
    }

    public class RequestIdListParameters
    {
        public int? StatusId { get; set; }
    }

    public partial class TicketListForSelect_Search
    {
        public int? EmployeeId { get; set; }

        [DefaultValue("")]
        public string? BranchId { get; set; }

        [DefaultValue("All")]
        public string? FilterType { get; set; }
    }
}
