using DigiDoc.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;


namespace DigiDoc.WebAPI.Helper
{
    public class ConfigurationReader
    {
        public static void readandsetConfig()
        {
            ConfigurationModel.ConnectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

            //try
            //{
            //    var settingList = new DapperHelper().ExecuteSP<SettingsList>("usp_GetSettingsList", ConfigurationModel.ConnectionString).ToList();
            //    if(settingList != null && settingList.Count > 0)
            //    {
            //        ConfigurationModel.Settings = settingList;
            //        LogHelper.Instance.Debug("settings value from the db assigned succesfully", "Read And Set Config", "Portal", "Initialization");
            //    }
            //    else
            //    {
            //        LogHelper.Instance.Log("Not able fetch or set the settings value from the db", "Read And Set Config", "Portal", "Initialization");
            //    }
            //}
            //catch(Exception ex)
            //{
            //    LogHelper.Instance.Error(ex, "Read And Set Config", "Portal", "Initialization");
            //}
        }
    }
}