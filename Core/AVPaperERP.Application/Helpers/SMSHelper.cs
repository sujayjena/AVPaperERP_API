using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using AVPaperERP.Application.Models;
using Newtonsoft.Json;
using AVPaperERP.Application.Interfaces;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace AVPaperERP.Helpers
{
    public interface ISMSHelper
    {
        Task<bool> SMSSend(SMS_Request parameterss);
        string SMSSend_SteviaDigital(string MobileNumber, string Message);
    }

    public class SMSHelper : ISMSHelper
    {
        private readonly ISMSConfigRepository _smsConfigRepository;

        public SMSHelper(ISMSConfigRepository smsConfigRepository)
        {
            _smsConfigRepository = smsConfigRepository;
        }

        public async Task<bool> SMSSend(SMS_Request parameters)
        {
            bool result = false;

            var vSMSConfig_Response = new SMSConfig_Response();

            try
            {
                string SMS_AuthKey = String.Empty;
                string SMS_SenderId = String.Empty;
                string SMS_BaseUrl = String.Empty;

                var vSMSConfig_Search = new SMSConfig_Search() { };
                var vSMSConfigObj = _smsConfigRepository.GetSMSConfigList(vSMSConfig_Search).Result.ToList().Where(x => x.IsActive == true).FirstOrDefault();
                if (vSMSConfigObj != null)
                {
                    SMS_AuthKey = vSMSConfigObj.Sms_AuthKey;
                    SMS_SenderId = vSMSConfigObj.Sms_SenderId;
                    SMS_BaseUrl = vSMSConfigObj.Sms_Url;

                    //Your authentication key  
                    string authKey = SMS_AuthKey;

                    //Sender ID,While using route4 sender id should be 6 characters long.  
                    string senderId = SMS_SenderId;

                    //Multiple mobiles numbers separated by comma  
                    string mobileNumber = parameters.Mobile;

                    //Your message to send, Add URL encoding here.  
                    string message = HttpUtility.UrlEncode(parameters.TemplateContent);
                    //string route = "4";

                    //Prepare you post parameters  
                    StringBuilder sbPostData = new StringBuilder();
                    sbPostData.AppendFormat("auth={0}", authKey);
                    sbPostData.AppendFormat("&senderid={0}", senderId);
                    sbPostData.AppendFormat("&msisdn={0}", mobileNumber);
                    sbPostData.AppendFormat("&message={0}", message);


                    //Call Send SMS API
                    //string baseurl = "https://sms.steviadigital.com/API/sms-api.php?auth=xxxxx&senderid=xxxxx&msisdn=xxxxxx&message="+message;
                    string sendSMSUri = SMS_BaseUrl;

                    //Create HTTPWebrequest  
                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);

                    //Prepare and Add URL Encoded data  
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(sbPostData.ToString());

                    //Specify post method  
                    httpWReq.Method = "POST";
                    httpWReq.ContentType = "application/x-www-form-urlencoded";
                    httpWReq.ContentLength = data.Length;
                    using (Stream stream = httpWReq.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    //Get the response  
                    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseString = reader.ReadToEnd();

                    //Close the response  
                    reader.Close();

                    response.Close();

                    // string responseString = "{\"status\":\"success\",\"totalnumbers_sbmited\":1,\"campg_id\":"769208",\"logid\":\"65c5b995a549d\",\"code\":\"100\",\"ts\":\"2024-02-09 11:05:17\"}";

                    dynamic jsonResults = JsonConvert.DeserializeObject<dynamic>(responseString);

                    parameters.TemplateContent = parameters.TemplateContent;
                    parameters.Status = jsonResults.ContainsKey("status") ? jsonResults.status : string.Empty;
                    parameters.desc = jsonResults.ContainsKey("desc") ? jsonResults.desc : string.Empty;
                    parameters.TotalNumberSubmitted = jsonResults.ContainsKey("totalnumbers_sbmited") ? jsonResults.totalnumbers_sbmited : null;
                    parameters.CampgId = jsonResults.ContainsKey("campg_id") ? jsonResults.campg_id : null;
                    parameters.LogId = jsonResults.ContainsKey("logid") ? jsonResults.logid : string.Empty;
                    parameters.Code = jsonResults.ContainsKey("code") ? jsonResults.code : null;
                    parameters.ts = jsonResults.ContainsKey("ts") ? jsonResults.ts : string.Empty;

                    if (parameters.Status == "success")
                        result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                parameters.desc = ex.Message;
            }

            #region SMS Log History Save/Upate

            var vSMS_RequestObj = new SMS_Request()
            {
                Id = 0,
                Ref1_OTPId = parameters.Ref1_OTPId,
                Ref2_Other = parameters.Ref2_Other,
                TemplateName = parameters.TemplateName,
                Mobile = parameters.Mobile,
                TemplateContent = parameters.TemplateContent,
                Status = parameters.Status,
                TotalNumberSubmitted = parameters.TotalNumberSubmitted,
                CampgId = parameters.CampgId,
                LogId = parameters.LogId,
                Code = parameters.Code,
                ErrorMessage = parameters.desc,
            };

            int resultSmsHistory = await _smsConfigRepository.SaveSMSHistory(vSMS_RequestObj);

            #endregion

            return result;
        }

        public string SMSSend_SteviaDigital(string MobileNumber, string Message)
        {
            string strResponse = "";

            try
            {
                //Your authentication key  
                string authKey = "D!~9573TbvMxRzLq4";

                //Multiple mobiles numbers separated by comma  
                string mobileNumber = MobileNumber;

                //Sender ID,While using route4 sender id should be 6 characters long.  
                string senderId = "QSERVO";

                //Your message to send, Add URL encoding here.  
                string message = HttpUtility.UrlEncode(Message);

                WebClient client = new WebClient();
                string baseurl = "https://sms.steviadigital.com/API/sms-api.php?auth=" + authKey + "&senderid=" + senderId + "&msisdn=" + mobileNumber + "&message=" + message;
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                strResponse = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
            }

            return strResponse;
        }
    }
}