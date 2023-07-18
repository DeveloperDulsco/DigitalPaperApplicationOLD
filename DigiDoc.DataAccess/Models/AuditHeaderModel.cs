using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class AuditHeaderModel
    {
        public string AuditHeaderID { get; set; }
        public string ModuleName { get; set; }
        public string UserName { get; set; }
        public string ActionName { get; set; }
        public DateTime AuditDateTime { get; set; }
       
        public int TotalRecords { get; set; }
        public bool DetailsAvailable { get; set; }
        public string AuditTime { get; set; }
        public string AuditDate { get; set; }
    }
}
