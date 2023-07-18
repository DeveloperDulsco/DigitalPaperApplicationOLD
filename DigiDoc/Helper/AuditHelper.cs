using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DigiDoc.Helper
{
    public class AuditHelper
    {
        public static void InsertAuditLog(string PageName,string UserName,string AuditMessage,List<AuditJsonObject> jsonObjects = null)
        {
            try
            {
                var auditResponse = new DapperHelper().ExecuteSP<SPResponseModel>("Usp_InsertAuditDetails", ConfigurationModel.ConnectionString, new
                {
                    ModuleName = PageName,
                    UserName = UserName,
                    ActionName = AuditMessage,
                    ChangeJSON = (jsonObjects != null) ? JsonConvert.SerializeObject(jsonObjects) :  null
                }).ToList();
                if (auditResponse == null || string.IsNullOrEmpty(auditResponse.First().Result) || !auditResponse.First().Result.Equals("200"))
                {
                    LogHelper.Instance.Debug("Failled to update the audit log", "Validate Login", "Portal", "Login");
                }
            }
            catch(Exception ex)
            {
                LogHelper.Instance.Error(ex, "InsertAuditLog", "Portal", "Audit");
            }
        }
    }
}