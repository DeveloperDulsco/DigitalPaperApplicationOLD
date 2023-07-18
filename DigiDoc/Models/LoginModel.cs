using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace DigiDoc.Models
{
    
    public class LoginRequestModel
    {
        //[ConfigurationProperty("maxRequestLength", DefaultValue = 1)]
        //[ConfigurationProperty("maxQueryStringLength", DefaultValue = 2048)]
        //[IntegerValidator(MinValue = 0)]
        
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username field is expecting Min. 3 & Max. 30 characters.")]
        public string UserName { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Password field is expecting Min. 3 & Max. 30 characters.")]
        public string UserPassword { get; set; }
        public string UserID { get; set; }
        public string SecuirityQuestion { get; set; }
        [System.Configuration.ConfigurationProperty("maxQueryStringLength", DefaultValue = 50)]
        [System.Configuration.IntegerValidator(MinValue = 0)]
        public string SecurityAnswer { get; set; }

    }
}
