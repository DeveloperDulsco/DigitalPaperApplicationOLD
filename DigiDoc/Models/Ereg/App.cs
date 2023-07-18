using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DigiDoc.Models.Ereg
{
    public class AppDetails
    {
        public int AppId { get; set; }
        public string AppName { get; set; }
        public string connectionId { get; set; }
        public bool IsAssinged { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
     }

    public class DigiMobileResponseModel
    {
        public bool result { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
        public string ResultCode { get; set; }
        public DataTable DataTable { get; set; }
    }
    
}