using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class  LoginUserDetailsModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public int PropertyID { get; set; } 
        public string PropertyName { get; set; }
        public int UserProfileID { get; set; }
        public string ProfileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModefiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? UpdatepasswordFlag { get; set; }
        public int TryCount { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
    }
}
