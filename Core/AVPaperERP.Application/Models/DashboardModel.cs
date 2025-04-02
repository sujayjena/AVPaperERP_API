using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class DashboardModel
    {
    }
    public class Dashboard_Search_Request
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int EmployeeId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }
    }

    public class Dashboard_TicketResolvedSummary_Result
    {
        public int? EngineerId { get; set; }
        public string EngineerName { get; set; }
        public int? Ticket { get; set; }
    }

    public class Dashboard_TicetStatusSummary_Search_Request
    {
        [DefaultValue(0)]
        public int CompanyId { get; set; }

        [DefaultValue("")]
        public string BranchId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [DefaultValue(0)]
        public int EmployeeId { get; set; }

        [DefaultValue("All")]
        public string FilterType { get; set; }

        [DefaultValue(true)]
        public bool? IsFiveDaysFilter { get; set; }

        [DefaultValue(null)]
        public bool? IsReopen { get; set; }
    }
    public class Dashboard_TicetStatusSummary_Result
    {
        public int Id { get; set; }
        public string TicketStatus { get; set; }
        public int? Ticket { get; set; }
    }

    public class Dashboard_TicketVisitSummary_Result
    {
        public DateTime? VisitDate { get; set; }
        public int? EngineerId { get; set; }
        public string EngineerName { get; set; }
        public int? Visit { get; set; }
    }

    public class DashboardNPS_Search_Request
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [JsonIgnore]
        public int Total { get; set; }
    }

    public class Dashboard_SurveyNPSSummary_Response
    {
        public string? types { get; set; }
        public string? QuestionName { get; set; }

        [JsonIgnore]
        public long? NPS_Without_Perct { get; set; }
        public string? NPS { get; set; }
    }

    public class Dashboard_TicketTRCSummary_Response
    {
        public int? TotalCustomer { get; set; }
        public int? TotalTicket { get; set; }
        public int? OpenTicket { get; set; }
        public int? ClosedTicket { get; set; }
        public int? TotalTRC { get; set; }
        public int? OpenTRC { get; set; }
        public int? ClosedTRC { get; set; }
    }
}
