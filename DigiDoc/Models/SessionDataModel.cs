using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class SessionDataModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string ProfileID { get; set; }
        public string ProfileName { get; set; }
        public string MenuName { get; set; }
        public string SubMenu { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsComment { get; set; }
        public bool IsEditable { get; set; }
        public string PropertyID { get; set; }
        public string TimeZoneId { get; set; }
        public string Email { get; set; }
    }
}