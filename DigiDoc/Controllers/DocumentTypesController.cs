using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using DigiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class DocumentTypesController : BaseController
    {
        // GET: DocumentTypes
        public ActionResult Index(string Message = null, bool Success = false)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            sessionData.MenuName = "Masters";
            sessionData.SubMenu = "Document Types";
            Session["DigiDocData"] = sessionData;
            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Viewed document type master list");
            DocumentTypeMaster documentType=new DocumentTypeMaster();
           
                ViewBag.Message = Message;
                ViewBag.Success = Success;
         
            //var spResponse = Helper.UtilityHelper.getDocumentGroupList();
            //if(spResponse.result)
            //{
            //    ViewBag.Message = Message;
            //    ViewBag.Success = Success;
            //    return View((List<DigiDoc.Models.DocumentTypeMaster >)spResponse.ResponseData);
            //}
            //else
            //{
            //    ViewBag.Success = false;
            //    ViewBag.Message = $"Error : {spResponse.ResponseMessage} code - ({spResponse.ResultCode})";
            //    return View();
            //}
            return View(documentType);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateDocumentTypeMaster(DocumentTypeMaster documentType)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            sessionData.MenuName = "Masters";
            sessionData.SubMenu = "Document Types";
            Session["DigiDocData"] = sessionData;
           
            if (documentType != null && !string.IsNullOrEmpty(documentType.DocumentCode) && !string.IsNullOrEmpty(documentType.DocumentName))
            {
                if (documentType.DocumentCode.ToUpper() == documentType.DocumentName.ToUpper())
                {
                    return Json(new { Result = false, Message = "Type and code should be different", Success = false });

                   // return RedirectToAction("Index", new { Message = "Type and code should be different", Success = false });
                   // return View(documentType);
                }
                else
                {
                    if(documentType.IsActive==null)
                    {

                        documentType.IsActive = false;
                    }
                    var spRespone = UtilityHelper.updateDocumentTypeMaster(documentType);
                    if (spRespone.result)
                    {
                        if (!string.IsNullOrEmpty(documentType.DocumentID))
                        {
                            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Document type updated");
                            //return RedirectToAction("Index", new { Message = "Document type updated!", Success = true });
                            return Json(new { Result = true, Message = "Document type updated!", Success = true });
                            }
                        else
                        {
                            AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "New Document type created");

                            //  return RedirectToAction("Index", new { Message = "Document type created!", Success = true });
                            return Json(new { Result = true, Message = "Document type created!", Success = true });

                        }
                    }
                    else
                    {
                        if (spRespone.ResultCode.Equals("212"))
                        {
                            return Json(new { Result = false, Message = spRespone.ResponseMessage, Success = false });

                          //  return RedirectToAction("Index", new { Message = spRespone.ResponseMessage, Success = false });
                        }
                        else
                        {
                            return Json(new { Result = false, Message = $"Error : {spRespone.ResponseMessage} , code - {spRespone.ResultCode}", Success = false });

                            //return RedirectToAction("Index", new { Message = $"Error : {spRespone.ResponseMessage} , code - {spRespone.ResultCode}", Success = false });
                        }
                    }
                }
            }
            else
            {
                ViewBag.Message = "Mandatory fields are missing";
                //return RedirectToAction("Index", new { Message = "Mandatory fields are missing", Success = false });
                //ModelState.AddModelError("Message", $"Mandatory fields are missing");
                return Json(new { Result = false, Message = $"Mandatory fields are missing", Success = false });
                //return View("Index", documentType);
            }
        }

        public ActionResult GetDocListAjax(DocumentDataTableModel model, Search search)
        {

            int start = 0;

            if (model.Start > 0)
            {
                start = model.Start / model.Length;
            }

            start += 1;

            string filterby = string.Empty;
            string soryOrder = "DEC";
            string sortBy = "";
            string sortColumn = "";

            if (Request.Params["search[value]"] != null)
            {
                filterby = Request.Params["search[value]"].ToString();
            }



            if (Request.Params["order[0][column]"] != null)
            {
                sortBy = Request.Params["order[0][column]"].ToString();

                if (sortBy == "0")
                {
                    sortColumn = "Serial #";
                }
                else if (sortBy == "1")
                {
                    sortColumn = "Document Type Code";
                }
                else if (sortBy == "2")
                {
                    sortColumn = "Document Type";
                }
                else if(sortBy == "3")
                {
                    sortColumn = "Status";
                }
                else
                {
                    sortColumn = "Serial #";
                }


            }

                if (Request.Params["order[0][dir]"] != null)
                {
                    soryOrder = Request.Params["order[0][dir]"].ToString();
                }
                var spResponse = UtilityHelper.getDocumentMasterList(start, model.Length, filterby, sortColumn, soryOrder);
            
                if (spResponse != null && spResponse.result)
                {
                    var documentList = (List<DocumentTypeMaster>)spResponse.ResponseData;
                    var TotalCount = documentList.FirstOrDefault().TotalRecords;

                    var response = new
                    {
                        draw = model.draw,
                        data = documentList,
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
        [HttpPost]
        public ActionResult SetdocumentActive(DocumentTypeMaster documentType)
        {


            if (documentType.DocumentID != null)
            {
                var spResponse = UtilityHelper.updateDocumentTypeMaster(new DocumentTypeMaster()
                {

                    IsActive = false,
                    DocumentID = documentType.DocumentID.ToString(),
                    
                    
                });
                if (spResponse != null && spResponse.result)
                {
                    LogHelper.Instance.Debug($"Document - {documentType.DocumentName}, document got deleted", " SetdocumentActive", "Portal", "SetdocumentActive");
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "Document Types";
                    Session["DigiDocData"] = sessionData;
                    AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", "Document type deleted", new List<AuditJsonObject>() {
                        new AuditJsonObject()
                        {
                            FieldName = "IsActive",
                            NewValue = true.ToString(),
                            OldValue = false.ToString()
                        }

                        });


                    return RedirectToAction("Index", "DocumentTypes",  new { Message = $" Document - {documentType.DocumentName} Successfully Deactivated", Success = true, });
                }
                else if (spResponse != null)
                {
                    if (spResponse.result)
                    {
                        LogHelper.Instance.Debug($"Document name- {documentType.DocumentName}, Delete document failled with reason {spResponse.ResponseMessage} (Error code - {spResponse.ResultCode})", "SetdocumentActive", "Portal", "SetdocumentActive");

                        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                        var sessionData = (SessionDataModel)Session["DigiDocData"];
                        sessionData.MenuName = "Masters";
                        sessionData.SubMenu = "Document Types";
                        Session["DigiDocData"] = sessionData;
                        AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update user ");

                        return RedirectToAction("Index", "DocumentTypes",  new { Message = $"Document - {documentType.DocumentName} Failed to Deactivate", Success = false });
                    }
                    else if (spResponse.ResultCode.Equals("201"))
                    {
                        return RedirectToAction("Index", "DocumentTypes", new { Message = spResponse.ResponseMessage, Success = false });
                    }
                    else if (spResponse.ResultCode.Equals("214"))
                    {
                        return RedirectToAction("Index", "DocumentTypes",  new { Message = $"Error : {spResponse.ResponseMessage} , code - {spResponse.ResultCode}", Success = false });
                    }
                    

                }
                else
                {
                    LogHelper.Instance.Debug($"document name- {documentType.DocumentName}, document delete failed", "SetdocumentActive", "Portal", "SetdocumentActive");

                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = "Masters";
                    sessionData.SubMenu = "Document Types";
                    Session["DigiDocData"] = sessionData;
                    AuditHelper.InsertAuditLog("Masters", sessionData != null ? sessionData.UserName : "", $"Failled to update document ");


                    return RedirectToAction("Index", "DocumentTypes",  new { Message = $"Failed to Update", Success = false });
                }
            }
            else
            {
                LogHelper.Instance.Debug($"document name- {documentType.DocumentName}, userid is null", "SetdocumentActive", "Portal", "SetdocumentActive");
                return RedirectToAction("Index", "DocumentTypes", new { Message = $"No Document Found", Success = false });

            }

            return RedirectToAction("Index", "DocumentTypes",  new { Message = $"No Document Found", Success = false });

        }
    }
}