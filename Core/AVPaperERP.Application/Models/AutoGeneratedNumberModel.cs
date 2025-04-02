using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPaperERP.Application.Models
{
    public class AutoGenerateNumber_Request
    {
        public string? Table { get; set; }
        public string? Prefix { get; set; }
        public int? Length { get; set; }
        public string? AutoNumber { get; set; }
    }
}
