using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDoc.PdfHelper.Model
{
    public class ResponseModel
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string ResponseCode { get; set; }
        public object Data { get; set; }
    }
}
