using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Models;

namespace DigiDoc.Helper
{
    public class ConfigurationReader
    {
        public static void readandsetConfig()
        {
            ConfigurationModel.ConnectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            ConfigurationModel.EmailAPIProxyHost = ConfigurationManager.AppSettings["EmailAPIProxyHost"].ToString();
            ConfigurationModel.EmailAPIProxyUN = ConfigurationManager.AppSettings["EmailAPIProxyUN"].ToString();
            ConfigurationModel.EmailAPIProxyPswd = ConfigurationManager.AppSettings["EmailAPIProxyPswd"].ToString();
            ConfigurationModel.isProxyEnableForEmailAPI = Convert.ToBoolean(ConfigurationManager.AppSettings["isProxyEnableForEmailAPI"].ToString());
            ConfigurationModel.EmailURL = ConfigurationManager.AppSettings["EmailURL"].ToString();
            ConfigurationModel.DigiDocURL = ConfigurationManager.AppSettings["DigiDocURL"].ToString();
            ConfigurationModel.DigiDocAPIProxyHost = ConfigurationManager.AppSettings["DigiDocAPIProxyHost"].ToString();
            ConfigurationModel.DigiDocAPIProxyUN = ConfigurationManager.AppSettings["DigiDocAPIProxyUN"].ToString();
            ConfigurationModel.DigiDocAPIProxyPswd = ConfigurationManager.AppSettings["DigiDocAPIProxyPswd"].ToString();
            ConfigurationModel.isProxyEnableForDigiDocAPI = Convert.ToBoolean(ConfigurationManager.AppSettings["isProxyEnableForDigiDocAPI"].ToString());
            

            try
            {
                var settingList = new DapperHelper().ExecuteSP<SettingsList>("usp_GetSettingsList", ConfigurationModel.ConnectionString).ToList();
                if(settingList != null && settingList.Count > 0)
                {
                    ConfigurationModel.Settings = settingList;
                    LogHelper.Instance.Debug("settings value from the db assigned succesfully", "Read And Set Config", "Portal", "Initialization");
                }
                else
                {
                    LogHelper.Instance.Log("Not able fetch or set the settings value from the db", "Read And Set Config", "Portal", "Initialization");
                }
            }
            catch(Exception ex)
            {
                LogHelper.Instance.Error(ex, "Read And Set Config", "Portal", "Initialization");
            }
        }
    }
}