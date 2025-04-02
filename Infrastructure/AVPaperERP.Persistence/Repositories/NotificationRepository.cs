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
    public class NotificationRepository : GenericRepository, INotificationRepository
    {
        private IConfiguration _configuration;

        public NotificationRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> SaveNotification(Notification_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@Subject", parameters.Subject);
            queryParameters.Add("@SendTo", parameters.SendTo);
            queryParameters.Add("@CustomerId", parameters.CustomerId);
            queryParameters.Add("@CustomerMessage", parameters.CustomerMessage);
            queryParameters.Add("@EmployeeId", parameters.EmployeeId);
            queryParameters.Add("@EmployeeMessage", parameters.EmployeeMessage);
            queryParameters.Add("@RefValue1", parameters.RefValue1);
            queryParameters.Add("@RefValue2", parameters.RefValue2);
            queryParameters.Add("@ReadUnread", parameters.ReadUnread);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveNotification", queryParameters);
        }

        public async Task<IEnumerable<Notification_Response>> GetNotificationList(Notification_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@NotifyDate", parameters.NotifyDate);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId == 0 ? parameters.UserId : SessionManager.LoggedInUserId);
            //queryParameters.Add("@UserId", parameters.UserId);

            var result = await ListByStoredProcedure<Notification_Response>("GetNotificationList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<Notification_Response?> GetNotificationById(int Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Id", Id);

            return (await ListByStoredProcedure<Notification_Response>("GetNotificationById", queryParameters)).FirstOrDefault();
        }
    }
}
