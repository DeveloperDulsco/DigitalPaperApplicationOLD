using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class SPUserResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }
}
