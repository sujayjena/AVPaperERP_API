using AVPaperERP.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<Dashboard_TicketResolvedSummary_Result>> GetDashboard_TicketResolvedSummary(Dashboard_Search_Request parameters);

        Task<IEnumerable<Dashboard_TicetStatusSummary_Result>> GetDashboard_TicetStatusSummary(Dashboard_TicetStatusSummary_Search_Request parameters);

        Task<IEnumerable<Dashboard_TicketVisitSummary_Result>> GetDashboard_TicketVisitSummary(Dashboard_Search_Request parameters);

        Task<IEnumerable<Dashboard_SurveyNPSSummary_Response>> GetDashboard_SurveyNPSSummary(DashboardNPS_Search_Request parameters);

        Task<IEnumerable<Dashboard_TicketTRCSummary_Response>> GetDashboard_TicketTRCSummary(Dashboard_Search_Request parameters);
    }
}
