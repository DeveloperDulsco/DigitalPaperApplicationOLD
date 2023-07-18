using DigiDoc.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class UserFormModel
    {
        public string IsUserEnabled { get; set; }
        public string PropertyName { get; set; }
        public string UserProfile { get; set; }
        [Required(ErrorMessage ="Email address is required",AllowEmptyStrings =false)]
        [RegularExpression(@"^([\w-\.])+@[a-zA-Z-]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "Email address is Not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage ="Username accepts only Alphabets")]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]

        public string UserName { get; set; }
        [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Name accepts only Alphabets")]

        public string RealName { get; set; }
        //[Required(ErrorMessage = "Phonenumber is required", AllowEmptyStrings = false)]
        
        [RegularExpression(@"^\(?([0-9]{2,3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4,9})$", ErrorMessage = "Phone number is not valid")]



        public string PhoneNumber { get; set; }
        public string UpdatePassword { get; set; }
        public string UpdateButon { get; set; }
        public string UpdatedPasswordFlag { get; set; }
        public string IsActive { get; set; }
        public int UserID { get; set; }
        [Required(ErrorMessage = "Password is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,16}$", ErrorMessage = "Password must contain 8 to 16 characters with 1 lower case, 1 upper case, 1 number,1 special character.")]

        public string UserPassword { get; set; }
       
        [Required(ErrorMessage = "Confirm Password is required", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*]).{8,16}$", ErrorMessage = "Password must contain 8 to 16 characters with 1 lower case, 1 upper case, 1 number,1 special character.")]
        [Compare("UserPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmUserPassword { get; set; }
        public string PropertyID { get; set; }
        public string UserProfileID { get; set; }
        public string SecuirityQuestion { get; set; }
        [Required(ErrorMessage = "Answer is required", AllowEmptyStrings = false)]
        public string SecurityAnswer { get; set; }

    }
    public class EditUserModel
    {
        public  UserModel user { get; set; }
        public List<UserProfileModel> userProfiles { get; set; }
        public List<PropertyMasterModel> propertyMasters { get; set; }
    }
}