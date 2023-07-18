using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.WebAPI.Models
{
    public class DocumentResponseModel
    {
     public string Result { get; set; }
     public string Message { get; set; }
     public string DocID { get; set; }
    public string DocumentDetailID { get; set; }
    }
}