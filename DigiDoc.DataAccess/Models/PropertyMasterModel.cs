using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.DataAccess.Models
{
    public class PropertyMasterModel
    {
        public string PropertyID { get; set; }
        public string PropertyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string WEBURI { get; set; }
        public bool PropertyStatus { get; set; }
        public byte[] PropertyLogo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
        public string TimeZone { get; set; }
    }
}
