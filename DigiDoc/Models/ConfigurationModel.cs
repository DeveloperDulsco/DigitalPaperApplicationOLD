using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigiDoc.DataAccess.Models;

namespace DigiDoc.Models
{
    public class ConfigurationModel
    {
        public static string ConnectionString {get;set;}
        public static List<SettingsList> Settings { get; set; }
        public static bool isProxyEnableForEmailAPI { get; set; }
        public static string EmailAPIProxyHost { get; set; }
        public static string EmailAPIProxyUN { get; set; }
        public static string EmailAPIProxyPswd { get; set; }
        public static bool isProxyEnableForDigiDocAPI { get; set; }
        public static string DigiDocAPIProxyHost { get; set; }
        public static string DigiDocAPIProxyUN { get; set; }
        public static string DigiDocAPIProxyPswd { get; set; }
        public static string DigiDocURL { get; set; }
        public static string EmailURL { get; set; }
    }

    
}