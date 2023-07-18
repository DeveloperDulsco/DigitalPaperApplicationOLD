using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class DocumentApprovalDetails
    {
        public string DocumentHeaderID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public bool? ApprovalStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? lastModifiedDate { get; set; }
        public string ApproverLevel { get; set; }
        public bool? RejectStatus { get; set; }
    }
}
