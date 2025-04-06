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
    public class SMSConfigRepository : GenericRepository, ISMSConfigRepository
    {
        private IConfiguration _configuration;

        public SMSConfigRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> SaveSMSConfig(SMSConfig_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@Sms_Name", parameters.Sms_Name);
            queryParameters.Add("@Sms_AuthKey", parameters.Sms_AuthKey);
            queryParameters.Add("@Sms_SenderId", parameters.Sms_SenderId);
            queryParameters.Add("@Sms_Url", parameters.Sms_Url);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveSMSCofig", queryParameters);
        }

        public async Task<IEnumerable<SMSConfig_Response>> GetSMSConfigList(SMSConfig_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<SMSConfig_Response>("GetSMSCofigList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<SMSConfig_Response?> GetSMSConfigById(int Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Id", Id);

            return (await ListByStoredProcedure<SMSConfig_Response>("GetSMSCofigById", queryParameters)).FirstOrDefault();
        }

        public async Task<int> SaveSMSHistory(SMS_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@Ref1_OTPId", parameters.Ref1_OTPId);
            queryParameters.Add("@Ref2_Other", parameters.Ref2_Other);
            queryParameters.Add("@TemplateName", parameters.TemplateName);
            queryParameters.Add("@Mobile", parameters.Mobile);
            queryParameters.Add("@TemplateContent", parameters.TemplateContent);
            queryParameters.Add("@Status", parameters.Status);
            queryParameters.Add("@TotalNumberSubmitted", parameters.TotalNumberSubmitted);
            queryParameters.Add("@CampgId", parameters.CampgId);
            queryParameters.Add("@LogId", parameters.LogId);
            queryParameters.Add("@Code", parameters.Code);
            queryParameters.Add("@ErrorMessage", parameters.ErrorMessage);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveSMSLogHistory", queryParameters);
        }

        public async Task<SMSHistory_Response?> GetSMSHistoryById(SMSHistory_Search parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();

            queryParameters.Add("@Ref1_OTPId", parameters.Ref1_OTPId);
            queryParameters.Add("@Ref2_Other", parameters.Ref2_Other);
            queryParameters.Add("@Template", parameters.TemplateName);

            return (await ListByStoredProcedure<SMSHistory_Response>("GetSMSHitoryById", queryParameters)).FirstOrDefault();
        }
    }
}
