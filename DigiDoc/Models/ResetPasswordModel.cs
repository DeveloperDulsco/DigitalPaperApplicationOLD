using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class ResetPasswordModel
    {
        [MaxLength(17)]
        [Required]
        public string NewPassword { get; set; }
        [MaxLength(17)]
        [Required]
        public string RetypedNewPassword { get; set; }
        [Required]
        public string SecuirityQuestion { get; set; }
        [MaxLength(100)]
        [Required]
        public string SecurityAnswer { get; set; }
        public string UserID { get; set; }
        public string ResetSubmitButon { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }
    }
}