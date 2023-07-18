using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using DigiDoc.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class ReportsController : BaseController
    {
        // GET: Reports
        
        public ActionResult AuditReport(string Message = null)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            sessionData.MenuName = "Reports";
            sessionData.SubMenu = "Audit Report";
            Session["DigiDocData"] = sessionData;
            var timezoneid = "Singapore Standard Time";
            if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
            {
                timezoneid = sessionData.TimeZoneId;
            }
            DateTime startDate = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
           
            ViewBag.StartDate = startDate.ToString("dd-MM-yyyy");
            ViewBag.EndDate = startDate.ToString("dd-MM-yyyy");
            ViewBag.Message = Message;
            return View();
        }
        [HttpPost]
        public ActionResult GetAuditDetails(int AuditID)
        {
            
            var spResponse = UtilityHelper.getAuditDetails(AuditID);
            if(spResponse != null && spResponse.result)
            {
                return PartialView((List<AuditDetailsModel>)spResponse.ResponseData);
            }
            else
                return PartialView();
        }

        [HttpPost]
        public ActionResult FetchAuditReport(DocumentDataTableModel model, Search search,string StartDate,string EndDate,string SubmitButon = null)
        {
            ViewBag.Message = null;
            if (string.IsNullOrEmpty(SubmitButon))
            {
                var sessionData = (SessionDataModel)Session["DigiDocData"];
                sessionData.MenuName = "Reports";
                sessionData.SubMenu = "Audit Report";
                Session["DigiDocData"] = sessionData;
                AuditHelper.InsertAuditLog("AuditReport", sessionData != null ? sessionData.UserName : "", $"Audit report was viewed by the user, for date range of ( {StartDate} - {EndDate}).");
                DateTime StartDateDT;
                DateTime EndDateDT;
                var timezoneid = "Singapore Standard Time";
                if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
                {
                    timezoneid = sessionData.TimeZoneId;
                }
                DateTime startDate = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
                if (string.IsNullOrEmpty(StartDate))
                    StartDateDT = startDate;
                else
                {
                    StartDateDT = DateTime.ParseExact(StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (string.IsNullOrEmpty(EndDate))
                    EndDateDT = startDate;
                else
                {
                    EndDateDT = DateTime.ParseExact(EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                int start = 0;

                if (model.Start > 0)
                {
                    start = model.Start / model.Length;
                }

                start += 1;

                string filterby = string.Empty;
                string soryOrder = "DESC";
                string sortBy = "";
                string sortColumn = "";

                if (Request.Params["search[value]"] != null)
                {
                    filterby = Request.Params["search[value]"].ToString();
                }



                if (Request.Params["order[0][column]"] != null)
                {
                    sortBy = Request.Params["order[0][column]"].ToString();
                }

                if (sortBy == "0")
                {
                    sortColumn = "Audit Date";
                }
                else if (sortBy == "1")
                {
                    sortColumn = "Audit Date";
                }
                else if (sortBy == "2")
                {
                    sortColumn = "Module Name";
                }
                else if (sortBy == "3")
                {
                    sortColumn = "User Name";
                }
                else if (sortBy == "4")
                {
                    sortColumn = "Action Name";
                }
                else if (sortBy == "5")
                {
                    sortColumn = "Audit Date";
                }


                if (Request.Params["order[0][dir]"] != null)
                {
                    soryOrder = Request.Params["order[0][dir]"].ToString();
                }
                
              
                var spResponse = UtilityHelper.fetchAuditHeaderDetails(start, model.Length, StartDateDT.ToString("yyyyMMdd"), EndDateDT.ToString("yyyyMMdd"), filterby,sortColumn,soryOrder,timezoneid);
                
                if (spResponse != null && spResponse.result)
                {
                    var auditHeaders = (List<AuditHeaderModel>)spResponse.ResponseData;
                    auditHeaders.ToList().ForEach(s => { 
                        
                        s.AuditDate=s.AuditDateTime.ToString("dd-MM-yyyy");

                        s.AuditTime = s.AuditDateTime.ToString("HH:mm:ss tt");
                    });
                   
                    var TotalCount = auditHeaders[0].TotalRecords;

                    var response = new
                    {
                        draw = model.draw,
                        data = auditHeaders,
                        recordsFiltered = TotalCount,
                        recordsTotal = TotalCount
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var response = new
                    {
                        draw = model.draw,

                        recordsFiltered = 0,
                        recordsTotal = 0
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                DateTime StartDateDT = DateTime.ParseExact(StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime EndDateDT = DateTime.ParseExact(EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                
                var sessionData = (SessionDataModel)Session["DigiDocData"];
               
                var timezoneid = "Singapore Standard Time";
                if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
                {
                    timezoneid = sessionData.TimeZoneId;
                }
                var SPResponse = UtilityHelper.getAuditDetailsForReport(StartDateDT.ToString("yyyyMMdd"), EndDateDT.ToString("yyyyMMdd"),timezoneid);
               
                if(SPResponse.result)
                {
                   
                
                   var response = PdfHelper.RDLCHelper.getAuditReportAsBase64(SPResponse.DataTable,new PdfHelper.Model.AuditReportParameterModel()
                    {
                        FromDate = StartDateDT.ToString("dd/MM/yyyy"),
                        ToDate = EndDateDT.ToString("dd/MM/yyyy"),
                        ReportName = "Audit Report"
                    });
                    if(response.Result)
                    {
                        byte[] pdfBytes = Convert.FromBase64String(response.Data.ToString());
                        return File(pdfBytes, "PDF", "DigiDocAuditReport.pdf");
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"Failled to generate audit report from RDLC with reason - {response.Message} (code : {response.ResponseCode})", "Fetch Audit report", "Portal", "Reports");
                        return RedirectToAction("AuditReport", new { Message = response.Message });
                    }
                }
                else
                {
                    LogHelper.Instance.Debug($"Failled to Fetch audit details from DB with reason - {SPResponse.ResponseMessage} (code : {SPResponse.ResultCode})", "Fetch Audit report", "Portal", "Reports");
                    return RedirectToAction("AuditReport", new { Message = SPResponse.ResponseMessage });
                }
            }
        }

        [HttpPost]
        public ActionResult GenerateAuditReport(string StartDate,string EndDate)
        {
            
            //else
            //{

            //}
            return Json(new { Result = true });
        }

    }
}