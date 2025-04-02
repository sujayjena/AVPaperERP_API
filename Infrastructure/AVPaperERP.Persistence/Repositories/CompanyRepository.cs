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
    public class CompanyRepository : GenericRepository, ICompanyRepository
    {
        private IConfiguration _configuration;

        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        #region Company 

        public async Task<int> SaveCompany(Company_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@CompanyName", parameters.CompanyName);
            queryParameters.Add("@CompanyTypeId", parameters.CompanyTypeId);
            queryParameters.Add("@RegistrationNumber", parameters.RegistrationNumber);
            queryParameters.Add("@ContactNumber", parameters.ContactNumber);
            queryParameters.Add("@Email", parameters.Email);
            queryParameters.Add("@Website", parameters.Website);
            queryParameters.Add("@TaxNumber", parameters.TaxNumber);
            queryParameters.Add("@AddressLine1", parameters.AddressLine1);
            queryParameters.Add("@AddressLine2", parameters.AddressLine2);
            queryParameters.Add("@StateId", parameters.StateId);
            queryParameters.Add("@DistrictId", parameters.DistrictId);
            queryParameters.Add("@CityId", parameters.CityId);
            queryParameters.Add("@Pincode", parameters.Pincode);
            queryParameters.Add("@GSTNumber", parameters.GSTNumber);
            queryParameters.Add("@PANNumber", parameters.PANNumber);
            queryParameters.Add("@LogoImageFileName", parameters.LogoImageFileName);
            queryParameters.Add("@LogoImageOriginalFileName", parameters.LogoImageOriginalFileName);
            queryParameters.Add("@NoofUserAdd", parameters.NoofUserAdd);
            queryParameters.Add("@NoofBranchAdd", parameters.NoofBranchAdd);
            queryParameters.Add("@AmcMonth", parameters.AmcMonth);
            queryParameters.Add("@AmcStartDate", parameters.AmcStartDate);
            queryParameters.Add("@AmcEndDate", parameters.AmcEndDate);
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveCompany", queryParameters);
        }

        public async Task<IEnumerable<Company_Response>> GetCompanyList(CompanySearch_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@SearchText", parameters.SearchText.SanitizeValue());
            queryParameters.Add("@IsActive", parameters.IsActive);
            queryParameters.Add("@PageNo", parameters.PageNo);
            queryParameters.Add("@PageSize", parameters.PageSize);
            queryParameters.Add("@Total", parameters.Total, null, System.Data.ParameterDirection.Output);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            var result = await ListByStoredProcedure<Company_Response>("GetCompanyList", queryParameters);
            parameters.Total = queryParameters.Get<int>("Total");

            return result;
        }

        public async Task<Company_Response?> GetCompanyById(int Id)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", Id);
            return (await ListByStoredProcedure<Company_Response>("GetCompanyById", queryParameters)).FirstOrDefault();
        }

        #endregion

        #region Company AMC

        public async Task<int> SaveAMCReminderEmail(CompanyAMCRminderEmail_Request parameters)
        {
            DynamicParameters queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", parameters.Id);
            queryParameters.Add("@CompanyId", parameters.CompanyId);
            queryParameters.Add("@AMCYear", parameters.AMCYear);
            queryParameters.Add("@AMCStartDate_EndDate_LastEmailDate", parameters.AMCStartDate_EndDate_LastEmailDate);
            queryParameters.Add("@AMCRemainingDays", parameters.AMCRemainingDays);
            queryParameters.Add("@AMCReminderCount", parameters.AMCReminderCount);
            queryParameters.Add("@AMCPreorPostExpire", parameters.AMCPreorPostExpire);
            queryParameters.Add("@AmcEndDate", parameters.AmcEndDate);
            queryParameters.Add("@AmcLastEmailDate", parameters.AmcLastEmailDate);
            queryParameters.Add("@UserId", SessionManager.LoggedInUserId);

            return await SaveByStoredProcedure<int>("SaveAMCReminderEmail", queryParameters);
        }

        #endregion
    }
}
