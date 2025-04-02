using AVPaperERP.Domain.Entities;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class SMSConfig_Search : BaseSearchEntity
    {
    }

    public class SMSConfig_Request : BaseEntity
    {
        public string Sms_Name { get; set; }
        public string Sms_AuthKey { get; set; }
        public string Sms_SenderId { get; set; }
        public string Sms_Url { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SMSConfig_Response : BaseResponseEntity
    {
        public string Sms_Name { get; set; }
        public string Sms_AuthKey { get; set; }
        public string Sms_SenderId { get; set; }
        public string Sms_Url { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SMS_Request : BaseEntity
    {
        public int? Ref1_OTPId { get; set; }
        public string Ref2_Other { get; set; }
        public string? TemplateName { get; set; }
        public string? Mobile { get; set; }
        public string? TemplateContent { get; set; }
        public string? Status { get; set; }
        public string? desc { get; set; }
        public int? TotalNumberSubmitted { get; set; }
        public int? CampgId { get; set; }
        public string? LogId { get; set; }
        public int? Code { get; set; }
        public string? ts { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class SMS_Response : BaseResponseEntity
    {
        public int? Ref1_OTPId { get; set; }
        public string Ref2_Other { get; set; }
        public string? TemplateName { get; set; }
        public string? Mobile { get; set; }
        public string? TemplateContent { get; set; }
        public string? Status { get; set; }
        public string? desc { get; set; }
        public int? TotalNumberSubmitted { get; set; }
        public int? CampgId { get; set; }
        public string? LogId { get; set; }
        public int? Code { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class SMSHistory_Search
    {
        public int? Ref1_OTPId { get; set; }
        public string Ref2_Other { get; set; }
        public string? TemplateName { get; set; }
    }

    public class SMSHistory_Response : BaseResponseEntity
    {
        public int? Ref1_OTPId { get; set; }
        public string? Ref2_Other { get; set; }
        public string? TemplateName { get; set; }
        public string? Mobile { get; set; }
        public string? TemplateContent { get; set; }
        public string? Status { get; set; }
        public int? TotalNumberSubmitted { get; set; }
        public int? CampgId { get; set; }
        public string? LogId { get; set; }
        public int? Code { get; set; }
    }
}
