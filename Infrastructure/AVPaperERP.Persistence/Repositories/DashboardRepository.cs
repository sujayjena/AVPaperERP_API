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
    public class DashboardRepository : GenericRepository, IDashboardRepository
    {
        private IConfiguration _configuration;

        public DashboardRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<Dashboard_TicketResolvedSummary_Result>> GetDashboard_TicketResolvedSummary(Dashboard_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@BranchId", parameters.BranchId);
            queryParameters.Add("@FromDate", parameters.FromDate);
            queryParameters.Add("@ToDate", parameters.ToDate);
            queryParameters.Add("@FilterType", parameters.FilterType);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Dashboard_TicketResolvedSummary_Result>("GetDashboard_TicketResolvedSummary", queryParameters);

            return result;
        }

        public async Task<IEnumerable<Dashboard_TicetStatusSummary_Result>> GetDashboard_TicetStatusSummary(Dashboard_TicetStatusSummary_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@BranchId", parameters.BranchId);
            queryParameters.Add("@FromDate", parameters.FromDate);
            queryParameters.Add("@ToDate", parameters.ToDate);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@FilterType", parameters.FilterType);
            queryParameters.Add("@IsFiveDaysFilter", parameters.IsFiveDaysFilter);
            queryParameters.Add("@IsReopen", parameters.IsReopen);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Dashboard_TicetStatusSummary_Result>("GetDashboard_TicetStatusSummary", queryParameters);

            return result;
        }

        public async Task<IEnumerable<Dashboard_TicketVisitSummary_Result>> GetDashboard_TicketVisitSummary(Dashboard_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@BranchId", parameters.BranchId);
            queryParameters.Add("@FromDate", parameters.FromDate);
            queryParameters.Add("@ToDate", parameters.ToDate);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@FilterType", parameters.FilterType);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Dashboard_TicketVisitSummary_Result>("GetDashboard_TicetVisitSummary", queryParameters);

            return result;
        }

        public async Task<IEnumerable<Dashboard_SurveyNPSSummary_Response>> GetDashboard_SurveyNPSSummary(DashboardNPS_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@FromDate", parameters.FromDate);
            queryParameters.Add("@ToDate", parameters.ToDate);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Dashboard_SurveyNPSSummary_Response>("GetDashboard_SurveyNPSSummary", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");
           
            return result;
        }

        public async Task<IEnumerable<Dashboard_TicketTRCSummary_Response>> GetDashboard_TicketTRCSummary(Dashboard_Search_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@BranchId", parameters.BranchId);
            queryParameters.Add("@FromDate", parameters.FromDate);
            queryParameters.Add("@ToDate", parameters.ToDate);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@FilterType", parameters.FilterType);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Dashboard_TicketTRCSummary_Response>("GetDashboard_TicketTRCSummary", queryParameters);

            return result;
        }
    }
}
