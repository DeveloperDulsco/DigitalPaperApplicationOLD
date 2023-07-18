using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class UpdateUserDetailsModel
    {
        public int? UserID { get; set; }
        public int? TryCount { get; set; }
        public bool? IsActive { get; set; }
        public bool? UpdatepasswordFlag { get; set; }

        [System.Configuration.ConfigurationProperty("maxQueryStringLength", DefaultValue = 25)]
        [System.Configuration.IntegerValidator(MinValue = 0)]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RealName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserProfileID { get; set; }
        public bool update { get; set; }
    }
}