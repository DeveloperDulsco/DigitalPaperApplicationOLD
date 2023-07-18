using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    
   public class UpdateUserModel
    {
        public string IsUserEnabled { get; set; }
        public string PropertyName { get; set; }
        public string UserProfile { get; set; }
        [Required(ErrorMessage = "Email address is required", AllowEmptyStrings = false)]

        [RegularExpression(@"^([\w-\.])+@[a-zA-Z-]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "Email address is Not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username accepts only Alphabets")]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]

        public string UserName { get; set; }
        [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Name accepts only Alphabets")]

        public string RealName { get; set; }
        //[Required(ErrorMessage = "Phonenumber is required", AllowEmptyStrings = false)]
        
        //  [RegularExpression(@"^(\+\d{1,3}[- ]?)?\d{10,15}$", ErrorMessage = "Phone number is not valid")]
        [RegularExpression(@"^\(?([0-9]{2,3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4,9})$", ErrorMessage = "Phone number is not valid")]
      
        public string PhoneNumber { get; set; }
        public string UpdatePassword { get; set; }
        public string UpdateButon { get; set; }
        public string UpdatedPasswordFlag { get; set; }
        public string IsActive { get; set; }
        public int UserID { get; set; }
        
        public string UserPassword { get; set; }

        public string ConfirmUserPassword { get; set; }
        public string PropertyID { get; set; }
        public string UserProfileID { get; set; }
        public string SecuirityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string ProfileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? lastModifiedDate { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public int TryCount { get; set; }
        public int TotalRecords { get; set; }

    }

}