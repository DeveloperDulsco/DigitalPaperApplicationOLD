using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class UserProfileModel
    {
        public string UserProfileID { get; set; }
        public string ProfileName { get; set; }
        public int PropertyID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string ModuleMenuName { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string ParentMenu { get; set; }
        public bool Status { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string MenuIcon { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsComment { get; set; }
        public bool IsEditable { get; set; }

    }
}
