using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AVPaperERP.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly ICompanyRepository _companyRepository;
        private IFileManager _fileManager;
        private IEmailHelper _emailHelper;
        private readonly IWebHostEnvironment _environment;

        private readonly IConfigRefRepository _configRefRepository;

        public CompanyController(ICompanyRepository companyRepository, IFileManager fileManager, IEmailHelper emailHelper, IWebHostEnvironment environment, IConfigRefRepository configRefRepository)
        {
            _companyRepository = companyRepository;
            _fileManager = fileManager;
            _emailHelper = emailHelper;
            _environment = environment;

            _configRefRepository = configRefRepository;

            _response = new ResponseModel();
            _response.IsSuccess = true;
        }

        #region Company 

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveCompany(Company_Request parameters)
        {
            // Company Logo Upload
            if (parameters! != null && !string.IsNullOrWhiteSpace(parameters.LogoImage_Base64))
            {
                var vUploadFile = _fileManager.UploadDocumentsBase64ToFile(parameters.LogoImage_Base64, "\\Uploads\\Company\\", parameters.LogoImageOriginalFileName);

                if (!string.IsNullOrWhiteSpace(vUploadFile))
                {
                    parameters.LogoImageFileName = vUploadFile;
                }
            }

            int result = await _companyRepository.SaveCompany(parameters);

            if (result == (int)SaveOperationEnums.NoRecordExists)
            {
                _response.Message = "No record exists";
            }
            else if (result == (int)SaveOperationEnums.ReocrdExists)
            {
                _response.Message = "Record already exists";
            }
            else if (result == (int)SaveOperationEnums.NoResult)
            {
                _response.Message = "Something went wrong, please try again";
            }
            else
            {
                _response.Message = "Record details saved sucessfully";
            }

            _response.Id = result;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetCompanyList(CompanySearch_Request parameters)
        {
            IEnumerable<Company_Response> lstCompanys = await _companyRepository.GetCompanyList(parameters);
            _response.Data = lstCompanys.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetCompanyById(int Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _companyRepository.GetCompanyById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        #endregion

        #region Company AMC

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> CheckCompanyAMC(CompanyAMC_Search parameters)
        {
            var vCompanySearch_Request = new CompanySearch_Request()
            {
                CompanyId = parameters.CompanyId,
            };

            var lstCompanys = await _companyRepository.GetCompanyList(vCompanySearch_Request);

            foreach (var companyItem in lstCompanys.ToList())
            {
                string sCompanyName = companyItem.CompanyName;
                int iCompanyId = companyItem.Id;
                string sAMCStartDate_EndDate_LastEmailDate = (companyItem.AmcStartDate.HasValue ? companyItem.AmcStartDate.Value.Date.ToString() : "") + " - " + (companyItem.AmcEndDate.HasValue ? companyItem.AmcEndDate.Value.Date.ToString() : "") + " - " + (companyItem.AmcLastEmailDate.HasValue ? companyItem.AmcLastEmailDate.Value.Date.ToString() : "");
                int iTotalAmcRemainingDays = Convert.ToInt32(companyItem.TotalAmcRemainingDays);

                var vCompanyAMCRminderEmail_RequestObj = new CompanyAMCRminderEmail_Request()
                {
                    CompanyId = iCompanyId,
                    AMCYear = (companyItem.AmcStartDate.HasValue ? companyItem.AmcStartDate.Value.Date.Year.ToString() : "") + "-" + (companyItem.AmcEndDate.HasValue ? companyItem.AmcEndDate.Value.Date.Year.ToString() : ""),
                    AMCStartDate_EndDate_LastEmailDate = sAMCStartDate_EndDate_LastEmailDate,
                    AMCRemainingDays = iTotalAmcRemainingDays,
                    AMCReminderCount = 1,

                    AMCPreorPostExpire = iTotalAmcRemainingDays == 0 ? true : false, // False = Pre Expire , True - Post Expire
                    AmcEndDate = companyItem.AmcEndDate,
                    AmcLastEmailDate = companyItem.AmcLastEmailDate,
                };

                // Save AMC Reminder
                int result = await _companyRepository.SaveAMCReminderEmail(vCompanyAMCRminderEmail_RequestObj);
                if (result > 0)
                {
                    vCompanyAMCRminderEmail_RequestObj.Id = result;

                    var vEmailCustomer = await SendAMCEmailToCustomer(companyItem, vCompanyAMCRminderEmail_RequestObj);
                    var vEmailServiceProvider = await SendAMCEmailToServiceProvider(companyItem, vCompanyAMCRminderEmail_RequestObj);

                    _response.Message = "AMC reminder sent sucessfully";
                }
                else
                {
                    _response.Message = "AMC reminder already sent!";
                }
            }

            return _response;
        }

        protected async Task<bool> SendAMCEmailToCustomer(Company_Response company_Response, CompanyAMCRminderEmail_Request companyAMCRminderEmail_Request)
        {
            bool result = false;
            string templateFilePath = "", emailTemplateContent = "", remarks = "", sSubjectDynamicContent = "";

            try
            {
                var vConfigRef_Search = new ConfigRef_Search()
                {
                    Ref_Type = "Email",
                    Ref_Param = "AMCReminderEmailToCustomer"
                };

                var vConfigRefObj = _configRefRepository.GetConfigRefList(vConfigRef_Search).Result.ToList().FirstOrDefault();
                if (vConfigRefObj != null)
                {
                    templateFilePath = _environment.ContentRootPath + "\\EmailTemplates\\AMC_Template.html";
                    emailTemplateContent = System.IO.File.ReadAllText(templateFilePath);

                    if (vConfigRefObj.Ref_Value1.IndexOf("[X]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sSubjectDynamicContent = vConfigRefObj.Ref_Value1.Replace("[X]", Convert.ToString(Convert.ToInt32(companyAMCRminderEmail_Request.AMCRemainingDays)));
                    }

                    if (emailTemplateContent.IndexOf("[ExpirationDate]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[ExpirationDate]", Convert.ToDateTime(company_Response.AmcEndDate).ToString("dd/MM/yyyy"));
                    }

                    if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", company_Response.CompanyLogoImageURL);
                    }

                    remarks = "AMC ReminderId- " + companyAMCRminderEmail_Request.Id;
                    result = await _emailHelper.SendEmail(module: vConfigRefObj.Ref_Param, subject: sSubjectDynamicContent, sendTo: "Customer", content: emailTemplateContent, recipientEmail: vConfigRefObj.Ref_Value2, files: null, remarks: remarks);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        protected async Task<bool> SendAMCEmailToServiceProvider(Company_Response company_Response, CompanyAMCRminderEmail_Request companyAMCRminderEmail_Request)
        {
            bool result = false;
            string templateFilePath = "", emailTemplateContent = "", remarks = "", sSubjectDynamicContent = "";

            try
            {
                var vConfigRefSP_Search = new ConfigRef_Search()
                {
                    Ref_Type = "Email",
                    Ref_Param = "AMCReminderEmailToVendor"
                };

                var vConfigRefSPObj = _configRefRepository.GetConfigRefList(vConfigRefSP_Search).Result.ToList().FirstOrDefault();
                if (vConfigRefSPObj != null)
                {
                    templateFilePath = _environment.ContentRootPath + "\\EmailTemplates\\AMC_Template.html";
                    emailTemplateContent = System.IO.File.ReadAllText(templateFilePath);

                    if (vConfigRefSPObj.Ref_Value1.IndexOf("[X]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        sSubjectDynamicContent = vConfigRefSPObj.Ref_Value1.Replace("[X]", Convert.ToString(Convert.ToInt32(companyAMCRminderEmail_Request.AMCRemainingDays)));
                    }

                    if (emailTemplateContent.IndexOf("[ExpirationDate]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[ExpirationDate]", Convert.ToDateTime(company_Response.AmcEndDate).ToString("dd/MM/yyyy"));
                    }

                    if (emailTemplateContent.IndexOf("[SenderCompanyLogo]", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        emailTemplateContent = emailTemplateContent.Replace("[SenderCompanyLogo]", company_Response.CompanyLogoImageURL);
                    }

                    remarks = "AMC ReminderId- " + companyAMCRminderEmail_Request.Id;
                    result = await _emailHelper.SendEmail(module: vConfigRefSPObj.Ref_Param, subject: sSubjectDynamicContent, sendTo: "Vendor", content: emailTemplateContent, recipientEmail: vConfigRefSPObj.Ref_Value2, files: null, remarks: remarks);
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        #endregion

    }
}
