using AVPaperERP.Domain.Entities;
using AVPaperERP.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{

    public class Notification_Search : BaseSearchEntity
    {
        public DateTime? NotifyDate { get; set; }
        [JsonIgnore]
        public int? UserId { get; set; }
    }

    public class Notification_Request : BaseEntity
    {
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerMessage { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeMessage { get; set; }
        public string RefValue1 { get; set; }
        public string RefValue2 { get; set; }
        public bool? ReadUnread { get; set; }
    }

    public class Notification_Response
    {
        public int Id { get; set; }
        public int? CustomerEmployeeId { get; set; }

        public string Subject { get; set; }
        public string SendTo { get; set; }
        public string Message { get; set; }
        public string RefValue1 { get; set; }
        public string RefValue2 { get; set; }

        [DefaultValue(false)]
        public bool? ReadUnread { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class NotificationPopup_Response
    {
        public NotificationPopup_Response()
        {
            NotificationList = new List<Notification_Response>();
        }

        [DefaultValue(0)]
        public int? UnReadCount { get; set; }

        public List<Notification_Response> NotificationList { get; set; }
    }
}
