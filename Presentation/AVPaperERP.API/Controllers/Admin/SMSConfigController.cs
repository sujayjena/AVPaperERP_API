using AVPaperERP.Application.Enums;
using AVPaperERP.Application.Helpers;
using AVPaperERP.Application.Interfaces;
using AVPaperERP.Application.Models;
using AVPaperERP.Helpers;
using AVPaperERP.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AVPaperERP.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSConfigController : CustomBaseController
    {
        private ResponseModel _response;
        private readonly ISMSConfigRepository _smsConfigRepository;
        private readonly IConfigRefRepository _configRefRepository;
        private ISMSHelper _smsHelper;

        public SMSConfigController(ISMSConfigRepository smsConfigRepository, IConfigRefRepository configRefRepository, ISMSHelper smsHelper)
        {
            _smsConfigRepository = smsConfigRepository;
            _response = new ResponseModel();
            _response.IsSuccess = true;
            _configRefRepository = configRefRepository;
            _smsHelper = smsHelper;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveSMSConfig(SMSConfig_Request parameters)
        {
            int result = await _smsConfigRepository.SaveSMSConfig(parameters);

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
        public async Task<ResponseModel> GetSMSConfigList(SMSConfig_Search parameters)
        {
            IEnumerable<SMSConfig_Response> lstRoles = await _smsConfigRepository.GetSMSConfigList(parameters);
            _response.Data = lstRoles.ToList();
            _response.Total = parameters.Total;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> GetSMSConfigById(int Id)
        {
            if (Id <= 0)
            {
                _response.Message = "Id is required";
            }
            else
            {
                var vResultObj = await _smsConfigRepository.GetSMSConfigById(Id);
                _response.Data = vResultObj;
            }
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> SaveSMSHistory(SMS_Request parameters)
        {
            int result = await _smsConfigRepository.SaveSMSHistory(parameters);

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
        public async Task<ResponseModel> GetSMSHistoryById(SMSHistory_Search parameters)
        {
            var vResultObj = await _smsConfigRepository.GetSMSHistoryById(parameters);
            _response.Data = vResultObj;
            return _response;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ResponseModel> TestSMS(SMS_Request parameters)
        {

            /*
             #region SMS Send

                    var vConfigRef_Search = new ConfigRef_Search()
                    {
                        Ref_Type = "SMS",
                        Ref_Param = "TicketGeneration"
                    };

                    string sSMSTemplateName = string.Empty;
                    string sSMSTemplateContent = string.Empty;
                    var vConfigRefObj = _configRefRepository.GetConfigRefList(vConfigRef_Search).Result.ToList().FirstOrDefault();
                    if (vConfigRefObj != null)
                    {
                        sSMSTemplateName = vConfigRefObj.Ref_Value1;
                        sSMSTemplateContent = vConfigRefObj.Ref_Value2;

                        if (!string.IsNullOrWhiteSpace(sSMSTemplateContent))
                        {
                            //Replace parameter 
                            sSMSTemplateContent = sSMSTemplateContent.Replace("{#var#}", parameters.MobileNumber);
                        }
                    }

                    var vsmsRequest = new SMS_Request()
                    {
                        OTPId = resultOTP,
                        TemplateName = sSMSTemplateName,
                        TemplateContent = sSMSTemplateContent,
                        Mobile = parameters.MobileNumber,
                    };

                    bool bSMSResult = await _smsHelper.SMSSend(vsmsRequest);

               #endregion
             */


            var vConfigRef_Search = new ConfigRef_Search()
            {
                Ref_Type = "SMS",
                Ref_Param = "TicketGeneration"
            };

            string sSMSTemplateName = string.Empty;
            string sSMSTemplateContent = string.Empty;
            var vConfigRefObj = _configRefRepository.GetConfigRefList(vConfigRef_Search).Result.ToList().FirstOrDefault();
            if (vConfigRefObj != null)
            {
                sSMSTemplateName = vConfigRefObj.Ref_Value1;
                sSMSTemplateContent = vConfigRefObj.Ref_Value2;

                if (!string.IsNullOrWhiteSpace(sSMSTemplateContent))
                {
                    //Replace parameter 
                    sSMSTemplateContent = sSMSTemplateContent.Replace("{#var#}", parameters.Mobile);
                }
            }

            var vsmsRequest = new SMS_Request()
            {
                Ref1_OTPId = Utilities.GenerateRandomNumForOTP(),
                TemplateName = sSMSTemplateName,
                TemplateContent = sSMSTemplateContent,
                Mobile = parameters.Mobile,
            };

            bool bSMSResult = await _smsHelper.SMSSend(vsmsRequest);

            if (bSMSResult)
            {
                _response.IsSuccess = true;
                _response.Message = "Sms sent successfully";
            }

            return _response;
        }
    }
}
