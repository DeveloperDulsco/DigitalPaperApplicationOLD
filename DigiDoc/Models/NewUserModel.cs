using DigiDoc.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class NewUserModel
    {
        public List<SecurityQuestionModel> securityQuestions { get; set; }
        public List<UserProfileModel> userProfiles { get; set; }
        public List<PropertyMasterModel> propertyMasters { get; set; }
        public UserFormModel UserForm { get; set; }
    }
}