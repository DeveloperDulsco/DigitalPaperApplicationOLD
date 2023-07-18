using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class ApprovalResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }

        public int ModuleID { get; set; }
    }
}