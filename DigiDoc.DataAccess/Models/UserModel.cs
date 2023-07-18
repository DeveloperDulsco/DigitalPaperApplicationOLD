using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "Email address is required", AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public int UserProfileID { get; set; }
        public string ProfileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? lastModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? UpdatedPasswordFlag { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public int TryCount { get; set; }
        public int TotalRecords { get; set; }
        
    }
}
