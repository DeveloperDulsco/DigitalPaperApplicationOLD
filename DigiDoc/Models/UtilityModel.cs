using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class UtilityResponseModel
    {
        public bool result { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
        public string ResultCode { get; set; }
        public DataTable DataTable {get ; set; }
    }
}