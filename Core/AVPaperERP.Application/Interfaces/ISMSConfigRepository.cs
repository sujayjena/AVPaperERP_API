using AVPaperERP.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Interfaces
{
    public interface ISMSConfigRepository
    {
        Task<int> SaveSMSConfig(SMSConfig_Request parameters);

        Task<IEnumerable<SMSConfig_Response>> GetSMSConfigList(SMSConfig_Search parameters);

        Task<SMSConfig_Response?> GetSMSConfigById(int Id);

        Task<int> SaveSMSHistory(SMS_Request parameters);

        Task<SMSHistory_Response?> GetSMSHistoryById(SMSHistory_Search parameters);
    }
}
