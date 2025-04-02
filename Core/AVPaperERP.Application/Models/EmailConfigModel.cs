using AVPaperERP.Domain.Entities;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{ 
    public class EmailConfig_Search : BaseSearchEntity
    {
    }

    public class EmailConfig_Request : BaseEntity
    {
        public string SmtpServerName { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool? SmtpUseDefaultCredentials { get; set; }
        public bool? SmtpEnableSSL { get; set; }
        public int? SmtpPort { get; set; }
        public int? SmtpTimeout { get; set; }
        public string FromAddress { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailSenderCompanyLogo { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EmailConfig_Response : BaseResponseEntity
    {
        public string SmtpServerName { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool? SmtpUseDefaultCredentials { get; set; }
        public bool? SmtpEnableSSL { get; set; }
        public int? SmtpPort { get; set; }
        public int? SmtpTimeout { get; set; }
        public string FromAddress { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailSenderCompanyLogo { get; set; }
        public bool? IsActive { get; set; }
    }


    public class EmailNotification_Request : BaseEntity
    {
        public string Module { get; set; }
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public string Content { get; set; }
        public string EmailTo { get; set; }
        public string RefValue1 { get; set; }
        public string RefValue2 { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsSent { get; set; }
    }

    public class EmailNotification_Response : BaseResponseEntity
    {
        public string Module { get; set; }
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public string Content { get; set; }
        public string EmailTo { get; set; }
        public string RefValue1 { get; set; }
        public string RefValue2 { get; set; }

        [DefaultValue(false)]
        public bool IsSent { get; set; }
    }
}
