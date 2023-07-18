using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class SecurityQuestionModel
    {
        public string SecurityQuestionID { get; set; }
        public string SecurityQuestion { get; set; }
        public bool? IsActive { get; set; }
    }
}
