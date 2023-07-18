using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models.DPO
{
    public class DocumentDetailsDTO
    {
       public int DetailID{get;set;} 
        public  string UserID{get;set;}
        public string DocumentType { get; set; } = null;
        public bool IsPageReloaded { get; set; } = false;
        public string Message { get; set; } = null;
        public bool Isapproval { get; set; } = false;   
    }
}