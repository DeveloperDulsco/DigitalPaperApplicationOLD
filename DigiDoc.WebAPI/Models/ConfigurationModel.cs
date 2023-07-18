using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigiDoc.DataAccess.Models;

namespace DigiDoc.WebAPI.Models
{
    public class ConfigurationModel
    {
        public static string ConnectionString {get;set;}
        public static List<SettingsList> Settings { get; set; }
    }

    
}