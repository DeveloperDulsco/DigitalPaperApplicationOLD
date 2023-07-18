using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Models
{
    public class ServiceParameters
    {
   
        public bool isProxyEnableForEmailAPI { get; set; }
        public string EmailAPIProxyHost { get; set; }
        public string EmailAPIProxyUN { get; set; }
        public string EmailAPIProxyPswd { get; set; }
        public string EmailURL { get; set; }
        public string ClientID { get; set; }
        public bool isProxyEnableForDigiDocAPI { get; set; }
        public string DigiDocAPIProxyHost { get; set; }
        public string DigiDocAPIProxyUN { get; set; }
        public string DigiDocAPIProxyPswd { get; set; }
        public string DigiDocURL { get; set; }
    }
}