using Azure.Storage.Blobs;
using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Helper;
using DigiDoc.Models;
using DigiDoc.Models.DPO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DigiDoc.Controllers
{
    public class GuestDocumentsController : Controller
    {
        // GET: GuestDocuments
        public ActionResult Index(string ResId, string Message = null, bool Success = false,string Username=null)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            DocumentInfoModel document = new DocumentInfoModel();
            var reservationnumberdetails = new DapperHelper().ExecuteSP<ReservationData>("usp_GetLinkedReservation", ConfigurationModel.ConnectionString, new { ResId = ResId }).ToList();
            if (reservationnumberdetails.Count > 0)
            {
                document.DocumentFileName = reservationnumberdetails.FirstOrDefault().ReservationNumber;
            }
            ViewBag.ResId = ResId;
            ViewBag.Message = Message;
            ViewBag.Success = Success;
            var userdetails = Username;
            userdetails = ConfigurationManager.AppSettings["Username"] != null ? ConfigurationManager.AppSettings["Username"].ToString() : "Username";
            return View(document);
        }

        public JsonResult GetGuestDocumentListAjax(string DocumentType,string DocumentFileName, DocumentDataTableModel model, Search search, string CreatedDate)
        {
            DateTime StartDateDT;
            DateTime EndDateDT;
            var timezoneid = "Singapore Standard Time";
            timezoneid = ConfigurationManager.AppSettings["TimeZone"] != null ? ConfigurationManager.AppSettings["TimeZone"].ToString() : "Singapore Standard Time";
          
           
           
            if (string.IsNullOrEmpty(CreatedDate))
                StartDateDT = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
            else
            {
                StartDateDT = DateTime.ParseExact(CreatedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(CreatedDate))
                EndDateDT = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
            else
            {
                EndDateDT = DateTime.ParseExact(CreatedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
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
            }


            if (sortBy == "1")
            {
                sortColumn = "Document Type";
            }
            else if (sortBy == "2")
            {
                sortColumn = "Document Name";
            }
            else if (sortBy == "3")
            {
                sortColumn = "Created Date";
            }
            else if (sortBy == "4")
            {
                sortColumn = "Guest Name";
            }
            else if (sortBy == "5")
            {
                sortColumn = "Room No";
            }
            else if (sortBy == "6")
            {
                sortColumn = "Chek-In Date";
            }
            else if (sortBy == "7")
            {
                sortColumn = "Check-Out Date";
            }
            else if (sortBy == "8")
            {
                sortColumn = "Last Modified By";
            }
            else if (sortBy == "8")
            {
                sortColumn = "Approval Status";
            }
            else if (sortBy == "9")
            {
                sortColumn = "Approver";
            }
            if (Request.Params["order[0][dir]"] != null)
            {
                soryOrder = Request.Params["order[0][dir]"].ToString();
            }

            
            var documentList = new DapperHelper().ExecuteSP<DocumentFileSummaryModel>("Usp_GetUserDocumentFileSummary", ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString, new { PageNumber = start, PageSize = model.Length, search = filterby, DocumentType = DocumentType, SortBy = soryOrder, Sort = sortColumn, StartDate = string.IsNullOrEmpty(CreatedDate) ? null : StartDateDT.ToString("yyyyMMdd"), EndDate = string.IsNullOrEmpty(CreatedDate) ? null : EndDateDT.ToString("yyyyMMdd"), TimezoneId = timezoneid, DocumentFileName= DocumentFileName }).ToList();




            if (documentList != null && documentList.Count > 0)
            {
                documentList.ToList().ForEach(s => {
                    s.CreatedDate = s.CreatedDatetime.ToString("dd-MM-yyyy");

                    s.Createdtime = s.CreatedDatetime.ToString("hh:mm:ss tt");
                });
                var TotalCount = documentList != null ? documentList.First().TotalRecords : 1;

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
                var TotalCount = model.Length;

                var response = new
                {
                    draw = model.draw,

                    recordsFiltered = 0,
                    recordsTotal = 0
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }




        }
        [HttpGet]
        public async Task<ActionResult> DocumentDetails(DocumentDetailsDTO dTO)
        {
            int DetailID = dTO.DetailID; string UserID = dTO.UserID; string DocumentType = dTO.DocumentType; bool IsPageReloaded = dTO.IsPageReloaded; string Message = dTO.Message; bool Isapproval = dTO.Isapproval;
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            ViewBag.IsApproval = Isapproval;
            ViewBag.ResId = dTO.ResId;
            DocumentModel temp = new DocumentModel();
            var timezoneid = "Singapore Standard Time";
            timezoneid = ConfigurationManager.AppSettings["TimeZone"] != null ? ConfigurationManager.AppSettings["TimeZone"].ToString() : "Singapore Standard Time";

            string message = "";
            var documentDetailListResponse = UtilityHelper.getDocumentDetails(DetailID, UserID);
            if (documentDetailListResponse != null && documentDetailListResponse.result && documentDetailListResponse.ResponseData != null)
            {
                List<DocumentModel> documentdetails = (List<DocumentModel>)documentDetailListResponse.ResponseData;
                if (documentdetails != null && documentdetails.Count > 0)
                {
                    temp = documentdetails.Last();

                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                 


                    temp.UserID = UserID;
                    if (documentdetails.FirstOrDefault().SignatureFile != null && documentdetails.FirstOrDefault().SignatureFile.Length > 0)
                    {
                        temp.SignatureFileBase64 = "data:image/png;base64," + Convert.ToBase64String(documentdetails.FirstOrDefault().SignatureFile);

                        temp.IsSignaturePresent = true;
                    }
                    else
                    {
                        temp.IsSignaturePresent = false;
                    }

                    if (temp.DocumentFile != null && temp.DocumentFile.Length > 0)
                    {
                        temp.DocumentFileBase64 = Convert.ToBase64String(temp.DocumentFile);
                        var fileName = DateTime.Now.ToString("ddMMyyHHmmssfff");
                        //System.IO.File.WriteAllBytes(Server.MapPath($"~/temp/{fileName}.pdf"), temp.DocumentFile);
                        temp.DocumentFileName = $"{fileName}.pdf";

                    }

                    var spResponse = UtilityHelper.getApprovalDetailsForDocID(temp.DocumentHeaderID);
                    if (spResponse != null && spResponse.result)
                    {
                        ViewBag.ShowAprovalView = false;
                        List<DocumentApprovalDetails> documentApprovalDetails = (List<DocumentApprovalDetails>)spResponse.ResponseData;
                        if (documentApprovalDetails != null && documentApprovalDetails.Count > 0)
                        {
                            var applist = documentApprovalDetails.FindAll(x => x.DocumentHeaderID != null);
                            if (applist != null && applist.Count > 0)
                            {
                                ViewBag.ShowAprovalView = true;
                            }
                            var documentApprovalStatusFalseList = documentApprovalDetails.FindAll(x => x.ApprovalStatus != null && !x.ApprovalStatus.Value && x.RejectStatus == false);
                            if (documentApprovalStatusFalseList != null && documentApprovalStatusFalseList.Count > 0)
                            {
                                var approvalUserString = documentApprovalStatusFalseList.Count == 1 ?
                                                        ((documentApprovalStatusFalseList.First().UserName != null && documentApprovalStatusFalseList.First().UserName.Equals(sessionData.UserName)) ?
                                                            "" : $"from '{documentApprovalStatusFalseList.First().UserName}'") :
                                                        "from multiple users";
                                ViewBag.InfoMessage = $"This document is waiting for the approval {approvalUserString}!";
                                var userApprovalList = documentApprovalStatusFalseList.FindAll(y => y.UserID.Equals(sessionData.UserID));
                                if (userApprovalList != null && userApprovalList.Count > 0)
                                {
                                    ViewBag.ShowApprovalButon = true;

                                }
                                else
                                {
                                    ViewBag.ShowApprovalButon = false;
                                }
                            }
                            else
                            {
                                if (documentApprovalDetails != null)
                                {
                                    var isApprovalthreshold = UtilityHelper.VerifyDocumentApprovalThreshold(documentApprovalDetails.Count);
                                    if (isApprovalthreshold.result && (bool)isApprovalthreshold.ResponseData)
                                    {
                                        ViewBag.InfoMessage = $"Maximum number for signatory has reached";
                                        ViewBag.ShowApprovalButon = false;
                                    }
                                    else
                                        ViewBag.ShowApprovalButon = true;
                                }
                                else
                                {
                                    ViewBag.ShowApprovalButon = true;
                                }
                            }
                        }
                        else
                        {

                            ViewBag.ShowApprovalButon = true;
                        }
                    }
                    else
                    {
                        ViewBag.ShowApprovalButon = true;
                    }
                    List<CommentDetails> commentList = new DapperHelper().ExecuteSP<CommentDetails>("Usp_GetDocumentComments", ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString, new { DocumentDetailID = temp.DocumentDetailID }).ToList();
                    temp.CommentDetails = commentList;
                    ViewBag.DocumentType = DocumentType;
                    ViewBag.Message = Message;

                    List<DocumentTypeMaster> documentypemaster = new DapperHelper().ExecuteSP<Models.DocumentTypeMaster>("Usp_GetDocumentMaster", ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString).ToList();
                    if (documentypemaster.Count > 0)
                    {
                        ViewBag.DocumentList = documentypemaster.Where(x => x.DocumentCode != "PA").ToList();
                    }
                    spResponse = UtilityHelper.getUserListDetails();
                    if (spResponse.result && spResponse.ResponseData != null)
                    {
                        var ProfilespResponse = UtilityHelper.getUserProfileDetails();
                        if (ProfilespResponse != null && ProfilespResponse.result && ProfilespResponse.ResponseData != null)
                        {
                            List<UserProfileModel> userProfileModeuleList = (List<UserProfileModel>)ProfilespResponse.ResponseData;
                            List<UserModel> IteratedUserList = new List<UserModel>();
                            foreach (var user in (List<UserModel>)spResponse.ResponseData)
                            {
                                //if (user.UserID != Int32.Parse(sessionData.UserID) && user.IsActive != null && user.IsActive.Value)
                                {
                                    if (userProfileModeuleList.Find(x => x.IsComment && x.UserProfileID.Equals(user.UserProfileID.ToString())) != null)
                                    {
                                        IteratedUserList.Add(user);
                                    }
                                }
                            }
                            ViewBag.Userlist = IteratedUserList;
                        }
                        else
                        {
                            ViewBag.Userlist = (List<UserModel>)spResponse.ResponseData;
                        }

                    }
                    if (temp != null)
                    {
                        temp.CommentDetails.ToList().ForEach(s => s.CreatedDateTime = DateTimeHelper.ConvertFromUTC(s.CreatedDateTime, timezoneid));

                        temp.CreatedDatetime = DateTimeHelper.ConvertFromUTC(temp.CreatedDatetime, timezoneid);
                        temp.LastCommentedDateTime = DateTimeHelper.ConvertFromUTC(Convert.ToDateTime(temp.LastCommentedDateTime), timezoneid);

                    }
                    if (!IsPageReloaded)
                        AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Viewed document (document id:{temp.DocumentHeaderID})");
                    return View(temp);
                }
                else
                {
                    message = "Not able to fetch the document details!";
                }
            }
            else if (documentDetailListResponse != null)
            {
                message = $"Message : {documentDetailListResponse.ResponseMessage}, code : {documentDetailListResponse.ResultCode}";
            }
            else
            {
                message = "Not able to fetch the document details!";
            }
            return RedirectToAction("Index", new { DocumentType = DocumentType, Message = message });
        }
        public async Task<FileResult> GetPDFStream(int DocumentFile)
        {
            try
            {
                byte[] DocumentFiles = null;
                var filename = string.Empty;
                var documentDetailListResponse = UtilityHelper.getDocumentFile(DocumentFile);
                if (documentDetailListResponse != null && documentDetailListResponse.result && documentDetailListResponse.ResponseData != null)
                {
                    List<DocumentModel> documentdetails = (List<DocumentModel>)documentDetailListResponse.ResponseData;
                    if (documentdetails.Count > 0)
                    {
                        LogHelper.Instance.Debug($"Document pdf stream starting", "GetPDFStream", "Portal", "DocumentDetails");
                        DocumentFiles = documentdetails.FirstOrDefault().DocumentFile;
                        if (DocumentFiles != null && DocumentFiles.Length > 0)
                        {
                            LogHelper.Instance.Debug($"Document byte array is empty", "GetPDFStream", "Portal", "DocumentDetails");


                        }
                        else
                        {
                            LogHelper.Instance.Debug($"Get Connectiostring", "GetPDFStream", "Portal", "DocumentDetails");
                            string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
                            LogHelper.Instance.Debug($"Connectionstring value" + ConnectionString, "ProcessDocument", "PortalAPI", "ProcessDocument");

                            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                            LogHelper.Instance.Debug($"BlobServiceClient Connection", "GetPDFStream", "Portal", "DocumentDetails");
                            //var s = new BlobStorage().DownloadToStream(blobServiceClient, path, "document" + DocumentFile.ToString() + ".pdf").Result;
                            var downloadedblob = await new BlobStorage().DownloadBlob(blobServiceClient, Server.MapPath($"~/temp/" + "document" + DocumentFile.ToString() + ".pdf"), "document" + DocumentFile.ToString() + ".pdf");
                            LogHelper.Instance.Debug($"Document downloaded successfully" + "document" + downloadedblob.ToString(), "GetPDFStream", "Portal", "DocumentDetails");
                            byte[] filefromblob = new BlobStorage().FileToByteArray(Server.MapPath($"~/temp/" + "document" + DocumentFile.ToString() + ".pdf"));
                            if (filefromblob.Length > 0)
                            {
                                DocumentFiles = filefromblob;
                            }

                            //DocumentFiles = new BlobStorage().GetFileBlobAsync("document" + DocumentFile.ToString() + ".pdf", blobServiceClient).Result;

                        }
                        filename = documentdetails.FirstOrDefault().DocumentFileName;
                    }
                }
                MemoryStream Stream = new MemoryStream(DocumentFiles);
                Stream.Write(DocumentFiles, 0, DocumentFiles.Length);
                Stream.Position = 0;

                return File(Stream, "application/pdf");
                //return new FileContentResult(DocumentFiles, "application/pdf");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Exception" + ex, "GetPDFStream", "Portal", "DocumentDetails");
            }
            return null;
        }
        [HttpPost]
        public FileResult GetPDFApproverDetails(int DocumentHeaderID)
        {
            //using (MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    StringReader sr = new StringReader(GridHtml);
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            //    pdfDoc.Open();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //    pdfDoc.Close();
            //    return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            //}
            //var sessionData = (SessionDataModel)Session["DigiDocData"];
            //var timezoneid = "Singapore Standard Time";
            //if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
            //{
            //    timezoneid = sessionData.TimeZoneId;
            //}
            //var spResponse = UtilityHelper.GetApprovalDetailsForPDF(DocumentHeaderID);
            //if (spResponse != null && spResponse.result)
            //{

            //    var query = spResponse.DataTable.Rows.Cast<DataRow>().Select((r, i) => new { r, i });
            //    foreach (var row in query)
            //    {

            //        row.r["CreatedDateTime"] = DateTimeHelper.ConvertFromUTC(row.r["CreatedDateTime"], timezoneid);

            //    }
            var sessionData = (SessionDataModel)Session["DigiDocData"];
            var timezoneid = "Singapore Standard Time";
            if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
            {
                timezoneid = sessionData.TimeZoneId;
            }
            var spResponse = UtilityHelper.GetApprovalDetails(DocumentHeaderID);

            if (spResponse != null && spResponse.result)
            {

                ViewBag.DocumentHeaderID = DocumentHeaderID;
                var approvalDocuments = (List<ApproverDetails>)spResponse.ResponseData;
                if (approvalDocuments != null && approvalDocuments.Count > 0)
                    approvalDocuments.ForEach(x => {

                        x.CreatedDateTime = DateTimeHelper.ConvertFromUTC(x.CreatedDateTime, timezoneid);



                    });
                ListtoDataTableConverter converter = new ListtoDataTableConverter();
                DataTable dt = converter.ToDataTable(approvalDocuments);
                ViewBag.DocumentHeaderID = DocumentHeaderID;
                //var approvalDocuments = (List<ApproverDetails>)spResponse.DataTable;
                //if (approvalDocuments != null && approvalDocuments.Count > 0)
                // spResponse.DataTable.ForEach(x => x.SignatureFileBase64 = "data:image/png;base64," + Convert.ToBase64String(x.SignatureFile));
                var response = PdfHelper.RDLCHelper.getCoverLetterBase64(dt, new PdfHelper.Model.CoverLetterParameterModel()
                {
                    ReportName = "Cover Letter"
                });
                if (response.Result)
                {
                    byte[] pdfBytes = Convert.FromBase64String(response.Data.ToString());

                    return File(pdfBytes, "PDF", "DigiDocCoverLetter.pdf");
                }
                else
                {
                    LogHelper.Instance.Debug($"Failled to generate audit report from RDLC with reason - {response.Message} (code : {response.ResponseCode})", "Fetch Audit report", "Portal", "Reports");
                    // return RedirectToAction("AuditReport", new { Message = response.Message });
                }

            }
            return null;
        }

    }
}