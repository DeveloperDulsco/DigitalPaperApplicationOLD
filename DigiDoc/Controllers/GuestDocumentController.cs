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
    [SessionCheck]
    public class GuestDocumentController :Controller
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
            if (string.IsNullOrEmpty(Username))
            {
                userdetails = ConfigurationManager.AppSettings["DefaultUser"] != null ? ConfigurationManager.AppSettings["DefaultUser"].ToString() : Username;
            }
            var loginStatus = new DapperHelper().ExecuteSP<LoginUserDetailsModel>("usp_ValidateUser", ConfigurationModel.ConnectionString, new { UserName = userdetails}).ToList();
            if (loginStatus != null && loginStatus.Count > 0 && !string.IsNullOrEmpty(loginStatus.First().Result) && loginStatus.First().Result.Equals("200"))
            {
                
                var timezoneid = "Singapore Standard Time";
                var sppropertresponse = UtilityHelper.getPropertyDetails(loginStatus.First().PropertyID.ToString());
                if (sppropertresponse != null)
                {
                    var propertlist = (List<PropertyMasterModel>)sppropertresponse.ResponseData;
                    timezoneid = propertlist.FirstOrDefault().TimeZone;

                }
                Session["GuestDigiDocData"] = new SessionDataModel()
                {
                    ProfileID = loginStatus.First().UserProfileID.ToString(),
                    ProfileName = loginStatus.First().ProfileName,
                    UserID = loginStatus.First().UserID,
                    UserName = loginStatus.First().UserName,
                    RealName = loginStatus.First().RealName,
                    PropertyID = loginStatus.First().PropertyID.ToString(),
                    TimeZoneId = timezoneid,
                    Email = loginStatus.First().Email
                };
                var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
                if (sessionData != null)
                {
                    var spResponse = UtilityHelper.getUserProfileDetails(sessionData.ProfileID);
                    if (spResponse != null && spResponse.result && spResponse.ResponseData != null)
                    {
                        var UserProfile = (List<UserProfileModel>)spResponse.ResponseData;
                        sessionData.IsEdit = UserProfile.FirstOrDefault().IsEdit;
                        sessionData.IsDelete = UserProfile.FirstOrDefault().IsDelete;
                        sessionData.IsPrint = UserProfile.FirstOrDefault().IsPrint;
                        sessionData.IsComment = UserProfile.FirstOrDefault().IsComment;
                        sessionData.IsEditable = UserProfile.FirstOrDefault().IsEditable;
                        Session["GuestDigiDocData"] = sessionData;
                        
                    }
                }
                AuditHelper.InsertAuditLog("GuestSearch", userdetails, "Succesfully logged-in");
                
             }
            else
            {
                return RedirectToAction("Index", "Login");
            }

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
                documentList.ToList().Where(x => x.DocumentCode != "RB");
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
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            ViewBag.IsApproval = Isapproval;
            
                ViewBag.ResId = dTO.ResId;
            DocumentModel temp = new DocumentModel();
            var timezoneid = "Singapore Standard Time";
            if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
            {
                timezoneid = sessionData.TimeZoneId;
            }
            string message = "";
            var documentDetailListResponse = UtilityHelper.getDocumentDetails(DetailID, UserID);
            if (documentDetailListResponse != null && documentDetailListResponse.result && documentDetailListResponse.ResponseData != null)
            {
                List<DocumentModel> documentdetails = (List<DocumentModel>)documentDetailListResponse.ResponseData;
                if (documentdetails != null && documentdetails.Count > 0)
                {
                    temp = documentdetails.Last();

                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    //var sessionData = (SessionDataModel)Session["DigiDocData"];
                    sessionData.MenuName = controllerName;
                    sessionData.SubMenu = "Document List";
                    temp.IsComment = sessionData.IsComment;
                    temp.IsDelete = sessionData.IsDelete;
                    temp.IsEdit = sessionData.IsEdit;
                    temp.IsPrint = sessionData.IsPrint;
                    temp.IsNotRecycleBin = true;
                    if (documentdetails.Last().DocumentCode == "RB")
                    {

                        temp.IsNotRecycleBin = false;
                    }
                    Session["GuestDigiDocData"] = sessionData;


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
                                if (user.UserID != Int32.Parse(sessionData.UserID) && user.IsActive != null && user.IsActive.Value)
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
        [HttpPost]
        public async Task<ActionResult> SaveComments(CommentsRequestModel commentsRequest)
        {
            var spResponse = UtilityHelper.insertComments(commentsRequest.DocumentHeaderID, commentsRequest.DocumentDetailID, commentsRequest.Comments, commentsRequest.UserID, null);
            if (spResponse != null && spResponse.result)
            {
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
                sessionData.MenuName = controllerName;
                sessionData.SubMenu = "Document List";
                Session["GuestDigiDocData"] = sessionData;
                AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Comment added : {commentsRequest.Comments}");
                var timezoneid = "Singapore Standard Time";
                if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
                {
                    timezoneid = sessionData.TimeZoneId;
                }
                DateTime dt = DateTime.UtcNow;
                dt = DateTimeHelper.ConvertFromUTC(dt, timezoneid);

                return Json(new { Result = true, CommentDate = dt.ToString("dd/MM/yyyy hh:mm:ss tt") });
            }
            else
            {
                return Json(new { Result = false, Message = spResponse.ResponseMessage });
            }
        }

        public async Task<ActionResult> SaveDocumentInfo(DocumentInfoModel documentInfo)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            var spResponse = UtilityHelper.saveDocumentInfo(documentInfo.UpdatedDocumentType, documentInfo.UpdatedDocumentName, documentInfo.DocumentHeaderID, documentInfo.DocumentDetailID);
            if (spResponse != null && spResponse.result)
            {

                if (!documentInfo.DocumentType.Equals(documentInfo.UpdatedDocumentType) || ((!string.IsNullOrEmpty(documentInfo.DocumentName)) && !documentInfo.DocumentName.Equals(documentInfo.UpdatedDocumentName)))
                {
                    AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Document details got updated.",
                        new List<AuditJsonObject>() {
                        new AuditJsonObject()
                        {
                            FieldName = "Document Type",
                            NewValue = documentInfo.UpdatedDocumentType,
                            OldValue = documentInfo.DocumentType
                        },
                        new AuditJsonObject()
                        {
                             FieldName = "Document Name",
                             NewValue = documentInfo.UpdatedDocumentName,
                             OldValue = documentInfo.DocumentName
                        }
                        });
                }
            }
            else
            {
                return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, Message = spResponse.ResponseMessage, IsApproval = documentInfo.IsApproval });
            }
            return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, IsApproval = documentInfo.IsApproval });
        }
        [HttpPost]
        public async Task<ActionResult> SendToApproval(DocumentInfoModel documentInfo)
        {
            string BaseURL = ConfigurationManager.AppSettings["EmailURL"].ToString();
            string FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string DisplayFromEmail = ConfigurationManager.AppSettings["DisplayFromEmail"].ToString();
            string RejectedSubject = ConfigurationManager.AppSettings["DocumentSendtoApprovalSubject"].ToString();

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var approverUser = Newtonsoft.Json.JsonConvert.DeserializeObject<ApproverUser>(documentInfo.ApproverUser);
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            if (!string.IsNullOrEmpty(documentInfo.DocumentHeaderID))
            {
                var spResponse = UtilityHelper.insertApprovalStatus(Int32.Parse(documentInfo.DocumentHeaderID), null, approverUser.ApprUserID, false, false, Int32.Parse(sessionData.UserID));
                if (spResponse != null && spResponse.result)
                {
                    UserModel touser = new UserModel();
                    var userresponse = UtilityHelper.getUserById(Convert.ToInt32(approverUser.ApprUserID));
                    if (userresponse != null)
                    {
                        try
                        {
                            var tousers = (List<UserModel>)userresponse.ResponseData;
                            if (tousers.Count > 0)
                            {
                                touser = tousers.FirstOrDefault();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    //using (var httpClient = new HttpClient())
                    //{




                    //    EmailRequest emailRequest = new EmailRequest()

                    //    {
                    //        FromEmail = FromEmail,
                    //        displayFromEmail = DisplayFromEmail,
                    //        ToEmail = touser.Email,
                    //        Subject = RejectedSubject,
                    //        DocumentName = documentInfo.DocumentName,
                    //        UserName = touser.RealName,
                    //        ApproverName = sessionData.RealName,
                    //        EmailType=EmailType.SendToApproval

                    //    };

                    //    httpClient.BaseAddress = new Uri(BaseURL);

                    //    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(emailRequest);

                    //    httpClient.DefaultRequestHeaders.Clear();


                    //    HttpContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    //    LogHelper.Instance.Log($"sending email to digidoc user", $"SendToApproval", "DigiDoc", "Pre-SendToApproval");

                    //    HttpResponseMessage response = await httpClient.PostAsync($"SendEmail", requestContent);

                    //}

                    EmailResponse emailResponse = await new WSClientHelper().SendEmail(new EmailRequest()

                    {
                        FromEmail = FromEmail,
                        displayFromEmail = DisplayFromEmail,
                        ToEmail = touser.Email,
                        Subject = RejectedSubject,
                        DocumentName = documentInfo.DocumentName,
                        UserName = touser.RealName,
                        ApproverName = sessionData.RealName,
                        EmailType = EmailType.SendToApproval


                    }, "Sendtoapproval", new ServiceParameters { EmailAPIProxyHost = ConfigurationModel.EmailAPIProxyHost, EmailAPIProxyPswd = ConfigurationModel.EmailAPIProxyPswd, isProxyEnableForEmailAPI = ConfigurationModel.isProxyEnableForEmailAPI, EmailURL = ConfigurationModel.EmailURL });
                    AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Document (doc id - {documentInfo.DocumentHeaderID}) send for approval to the user {approverUser.ApprUserRealName}");
                }
                else
                {
                    LogHelper.Instance.Debug($"Failled to insert details for approval with reason ({spResponse.ResponseMessage} , code- {spResponse.ResultCode})", "Save Document Signature", "Portal", "Document");
                    return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, Message = spResponse.ResponseMessage, IsApproval = documentInfo.IsApproval });
                }
                return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, IsApproval = documentInfo.IsApproval });
            }
            else
            {
                LogHelper.Instance.Debug($"Failled to insert details for approval because document headerid is missing", "Save Document Signature", "Portal", "Document");
                return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.UpdatedDocumentType, IsPageReloaded = true, Message = "Failled to assign the document for approval", IsApproval = documentInfo.IsApproval });
            }

        }
        [HttpPost]

        public async Task<ActionResult> SaveDocumentSignature(DocumentInfoModel documentInfo)
        {
            LogHelper.Instance.Debug("Save DocumentSignature Started", "Save Document Signature", "Portal", "Document");
            string BaseURL = ConfigurationManager.AppSettings["EmailURL"].ToString();
            string FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string DisplayFromEmail = ConfigurationManager.AppSettings["DisplayFromEmail"].ToString();
            string RejectedSubject = ConfigurationManager.AppSettings["DocumentAcceptedSubject"].ToString();

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            if (!string.IsNullOrEmpty(documentInfo.Base64Signature) && !string.IsNullOrEmpty(documentInfo.DocumentHeaderID))
            {
                LogHelper.Instance.Debug("DocumentSignature" + documentInfo.Base64Signature, "Save Document Signature", "Portal", "Document");

                var spResponse = UtilityHelper.insertApprovalStatus(Int32.Parse(documentInfo.DocumentHeaderID), documentInfo.Base64Signature, documentInfo.UserID, true, false, 0);
                if (spResponse != null && spResponse.result)
                {
                    if (!string.IsNullOrEmpty(documentInfo.Comment))
                    {
                        spResponse = UtilityHelper.insertComments(Convert.ToInt32(documentInfo.DocumentHeaderID), Convert.ToInt32(documentInfo.DocumentDetailID), documentInfo.Comment, documentInfo.UserID, Convert.ToInt32(spResponse.ResponseData));
                        if (spResponse != null && spResponse.result)
                        {
                            #region Email
                            UserModel touser = new UserModel();
                            int sendersid = 0;
                            if (String.IsNullOrEmpty(documentInfo.SenderId) || documentInfo.SenderId == "0")
                            {
                                sendersid = documentInfo.LastCommentedUserID;
                            }
                            else
                            {
                                sendersid = Convert.ToInt32(documentInfo.SenderId);
                            }
                            var userresponse = UtilityHelper.getUserById(Convert.ToInt32(sendersid));
                            var tousers = (List<UserModel>)userresponse.ResponseData;
                            if (tousers != null)
                            {
                                touser = tousers.FirstOrDefault();
                            }
                            //using (var httpClient = new HttpClient())
                            //{



                            //    EmailRequest emailRequest = new EmailRequest()

                            //    {
                            //        FromEmail = FromEmail,
                            //        displayFromEmail = DisplayFromEmail,
                            //        ToEmail = sessionData.Email,
                            //        Subject = RejectedSubject,
                            //        DocumentName = documentInfo.DocumentName,
                            //        UserName = sessionData.RealName,
                            //        ApproverName = touser.RealName,
                            //         EmailType = EmailType.Accept

                            //    };

                            //    httpClient.BaseAddress = new Uri(BaseURL);

                            //    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(emailRequest);

                            //    httpClient.DefaultRequestHeaders.Clear();


                            //    HttpContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

                            //    LogHelper.Instance.Log($"sending email to digidoc user", $"SendToApproval", "DigiDoc", "Pre-SendToApproval");

                            //    HttpResponseMessage response = await httpClient.PostAsync($"SendEmail", requestContent);

                            //}

                            if (documentInfo.IsApproval)
                            {
                                EmailResponse emailResponse = await new WSClientHelper().SendEmail(new EmailRequest()

                                {
                                    FromEmail = FromEmail,
                                    displayFromEmail = DisplayFromEmail,
                                    ToEmail = touser.Email,
                                    Subject = RejectedSubject,
                                    DocumentName = documentInfo.DocumentName,
                                    UserName = touser.RealName,
                                    ApproverName = sessionData.RealName,
                                    EmailType = EmailType.Accept


                                }, "Sendtoapproval", new ServiceParameters { EmailAPIProxyHost = ConfigurationModel.EmailAPIProxyHost, EmailAPIProxyPswd = ConfigurationModel.EmailAPIProxyPswd, isProxyEnableForEmailAPI = ConfigurationModel.isProxyEnableForEmailAPI, EmailURL = ConfigurationModel.EmailURL });
                            }
                            #endregion
                            AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Comment added : {documentInfo.Comment}");
                        }
                        else
                        {
                            LogHelper.Instance.Debug($"Failled to comment of signature : ({spResponse.ResponseMessage})", "Save Document Signature", "Portal", "Document");

                        }
                    }
                    var timezoneid = "Singapore Standard Time";
                    if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
                    {
                        timezoneid = sessionData.TimeZoneId;
                    }
                    DateTime propertytimezone = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
                    AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Document approved by the user : {sessionData.RealName}");
                    //if (System.IO.File.Exists(Server.MapPath($"~/temp/{documentInfo.DocumentFileName}")))
                    //{
                    //    LogHelper.Instance.Debug("Temp File Path Exists", "Save Document Signature", "Portal", "Document");

                    //    try
                    //    {
                    //        byte[] bytes = System.IO.File.ReadAllBytes(ControllerContext.HttpContext.Server.MapPath($"~/temp/{documentInfo.DocumentFileName}"));
                    //        string PDFBase64 = Convert.ToBase64String(bytes);
                    //        // LogHelper.Instance.Debug($"Writing to PDF"+PDFBase64, "Save Document Signature", "Portal", "Document");

                    //        //PdfHelper.Model.ResponseModel response = PdfHelper.PdfHelper.aproveDocumentWithCoverletter(PDFBase64, documentInfo.Base64Signature, 100, 80, sessionData.RealName, propertytimezone, documentInfo.Comment);
                    //        //LogHelper.Instance.Debug("Pdf Response"+response.Data.ToString(), "Save Document Signature", "Portal", "Document");

                    //        //if (response != null && response.Result && response.Data != null)
                    //        //{
                    //        //    spResponse = UtilityHelper.updateDocument(System.Convert.FromBase64String(response.Data.ToString()), documentInfo.DocumentHeaderID, documentInfo.DocumentDetailID, documentInfo.DocumentType);
                    //        //    if (spResponse.result)
                    //        //    {
                    //        //        AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Signature stamped to the document successfully");
                    //        //    }
                    //        //    else
                    //        //    {
                    //        //        LogHelper.Instance.Debug($"Failled to stamp signature on PDF : ({spResponse.ResponseMessage})", "Save Document Signature", "Portal", "Document");
                    //        //    }
                    //        //}
                    //        //else if (response != null)
                    //        //{
                    //        //    LogHelper.Instance.Debug($"Failled to stamp signature on PDF : ({response.Message})", "Save Document Signature", "Portal", "Document");
                    //        //}
                    //        //else
                    //        //{
                    //        //    LogHelper.Instance.Debug("Failled to stamp signature on PDF", "Save Document Signature", "Portal", "Document");
                    //        //}
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        LogHelper.Instance.Error(ex, "Save Document Signature", "Portal", "Document");
                    //    }
                    //}
                    //else
                    //{
                    //    LogHelper.Instance.Debug("Document not Exists", "Save Document Signature", "Portal", "Document");


                    //}
                    return Json(new { Result = true, Message = "Success", DetailID = documentInfo.DocumentDetailID, DocumentType = documentInfo.DocumentType, IsApproval = documentInfo.IsApproval });
                }
                else
                {
                    return Json(new { Result = false, Message = spResponse.ResponseMessage, IsApproval = documentInfo.IsApproval });
                }
            }
            else
            {
                return Json(new { Result = false, Message = "Signature can not be blank", IsApproval = documentInfo.IsApproval });
            }

        }

        public async Task<ActionResult> ApprovalList()
        {
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = "Document";
            sessionData.SubMenu = "Approvals";
            Session["GuestDigiDocData"] = sessionData;
            return View();
        }


        [HttpPost]
        public ActionResult FetchDocumentApprovalList(DocumentDataTableModel model, Search search, string ApprovalStatus, string SubmitButon = null)
        {
            //if (string.IsNullOrEmpty(SubmitButon))
            {
                var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
                sessionData.MenuName = "Document";
                sessionData.SubMenu = "Approvals";
                Session["GuestDigiDocData"] = sessionData;
                bool tempStatus;
                bool status = !string.IsNullOrEmpty(ApprovalStatus) ?
                                        (bool.TryParse(ApprovalStatus, out tempStatus) ? tempStatus : false)
                                        : false;
                string statusMessage = status ? "Approved" : "Pending";
                AuditHelper.InsertAuditLog("AuditReport", sessionData != null ? sessionData.UserName : "", $"Fetching document approval list with status: {statusMessage}");


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

                if (sortBy == "0")
                {
                    sortColumn = "Document Type";
                }
                else if (sortBy == "1")
                {
                    sortColumn = "Document Name";
                }
                else if (sortBy == "2")
                {
                    sortColumn = "Approval Created Date";
                }
                else if (sortBy == "3")
                {
                    sortColumn = "Approved Date";
                }




                if (Request.Params["order[0][dir]"] != null)
                {
                    soryOrder = Request.Params["order[0][dir]"].ToString();
                }

                var timezoneid = "Singapore Standard Time";
                if (!string.IsNullOrEmpty(sessionData.TimeZoneId))
                {
                    timezoneid = sessionData.TimeZoneId;
                }

                var spResponse = UtilityHelper.fetchApprovalDocumentList(start, model.Length, status, Int32.Parse(sessionData.UserID), sortColumn, soryOrder, filterby);
                if (spResponse != null && spResponse.result)
                {

                    var approvalDocuments = (List<DocumentFileSummaryModel>)spResponse.ResponseData;
                    approvalDocuments.ToList().ForEach(s =>
                    {
                        s.CreatedDatetime = DateTimeHelper.ConvertFromUTC(s.CreatedDatetime, timezoneid);
                        s.CreatedDate = s.CreatedDatetime.ToString("dd-MM-yyyy");
                        if (s.ApprovalCreatedDateTime != DateTime.MinValue)
                        {
                            s.ApprovalCreatedDateTime = DateTimeHelper.ConvertFromUTC(s.ApprovalCreatedDateTime, timezoneid);
                            s.ApprovalCreatedDate = s.ApprovalCreatedDateTime.ToString("dd-MM-yyyy");
                        }
                        s.ApprovallastUpdatedDateTime = DateTimeHelper.ConvertFromUTC(Convert.ToDateTime(s.ApprovallastUpdatedDateTime), timezoneid);
                        s.ApprovallastUpdatedDate = Convert.ToDateTime(s.ApprovallastUpdatedDateTime).ToString("dd-MM-yyyy");
                        s.Createdtime = s.CreatedDatetime.ToString("hh:mm:ss tt");
                        s.ApprovalCreatedTime = s.ApprovalCreatedDateTime == DateTime.MinValue ? null : s.ApprovalCreatedDateTime.ToString("hh:mm:ss tt");
                        s.ApprovallastUpdatedTime = Convert.ToDateTime(s.ApprovallastUpdatedDateTime).ToString("hh:mm:ss tt");

                    });

                    int TotalCount = model.Length;
                    if (approvalDocuments != null && approvalDocuments.Count > 0)
                        TotalCount = approvalDocuments.First().TotalRecords;

                    var response = new
                    {
                        draw = model.draw,
                        data = approvalDocuments,
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

        }

        public async Task<ActionResult> ChangeDocumentType(DocumentInfoModel documentInfo)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            var spResponse = UtilityHelper.saveDocumentInfo(documentInfo.UpdatedDocumentType, documentInfo.UpdatedDocumentName, documentInfo.DocumentHeaderID, documentInfo.DocumentDetailID);
            if (spResponse != null && spResponse.result)
            {

                if (!documentInfo.DocumentType.Equals(documentInfo.UpdatedDocumentType))
                {
                    AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Document details got updated.",
                        new List<AuditJsonObject>() {
                        new AuditJsonObject()
                        {
                            FieldName = "Document Type",
                            NewValue = documentInfo.UpdatedDocumentType,
                            OldValue = documentInfo.DocumentType
                        }
                        });
                }
                return RedirectToAction("Index", new { DocumentType = documentInfo.DocumentType, Message = "Success", Success = true });
            }
            else
            {
                return RedirectToAction("Index", new { DocumentType = documentInfo.DocumentType, Message = "Error, please try again", Success = false });
            }

        }

        [HttpPost]
        public ActionResult CheckMaximumApproverLimitExceeded(int DocumentHeaderID)
        {
            bool countexceeded = false;
            string message = "";
            var spResponse = UtilityHelper.getApprovalDetailsForDocID(DocumentHeaderID.ToString());
            if (spResponse != null && spResponse.result)
            {
                List<DocumentApprovalDetails> documentApprovalDetails = (List<DocumentApprovalDetails>)spResponse.ResponseData;
                if (documentApprovalDetails != null && documentApprovalDetails.Count > 0)
                {

                    if (documentApprovalDetails != null)
                    {
                        var isApprovalthreshold = UtilityHelper.VerifyDocumentApprovalCount(documentApprovalDetails.Count);
                        if (isApprovalthreshold.result && (bool)isApprovalthreshold.ResponseData)
                        {
                            countexceeded = true;
                            message = "Maximum number for signatory has reached";
                        }
                    }

                }


            }
            var response = new
            {
                Result = countexceeded,
                Message = message
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public ActionResult GetApproverDetails(int DocumentHeaderID)
        {

            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
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
                        //x.SignatureFileBase64 = "data:image/png;base64," + Convert.ToBase64String(x.SignatureFile);
                        x.CreatedDateTime = DateTimeHelper.ConvertFromUTC(x.CreatedDateTime, timezoneid);



                    });
                return PartialView("DocumentSignature", approvalDocuments);
            }
            return PartialView("DocumentSignature", null);
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
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
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

        public async Task<ActionResult> ChangeMultipleDocumentType(string DocumentDetailIDs, string DocumentTypes, string UpdatedDocumentType)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            var s = DocumentDetailIDs.Split(',');
            foreach (var item in s)
            {
                var spResponse = UtilityHelper.saveDocumentInfo(UpdatedDocumentType, null, item, item);

            }

            return RedirectToAction("Index", new { DocumentType = DocumentTypes, Message = "Success", Success = true });


        }

        public async Task<ActionResult> RejectApproval(DocumentInfoModel documentInfo)
        {
            string BaseURL = ConfigurationManager.AppSettings["EmailURL"].ToString();
            string FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string DisplayFromEmail = ConfigurationManager.AppSettings["DisplayFromEmail"].ToString();
            string RejectedSubject = ConfigurationManager.AppSettings["DocumentRejectedSubject"].ToString();

            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
            sessionData.MenuName = controllerName;
            sessionData.SubMenu = "Document List";
            Session["GuestDigiDocData"] = sessionData;
            if (!string.IsNullOrEmpty(documentInfo.DocumentHeaderID))
            {
                var spResponse = UtilityHelper.insertApprovalStatus(Int32.Parse(documentInfo.DocumentHeaderID), null, documentInfo.UserID, false, true, 0);
                if (spResponse != null && spResponse.result)
                {
                    if (!string.IsNullOrEmpty(documentInfo.Comment))
                    {
                        spResponse = UtilityHelper.insertComments(Convert.ToInt32(documentInfo.DocumentHeaderID), Convert.ToInt32(documentInfo.DocumentDetailID), documentInfo.Comment, documentInfo.UserID, Convert.ToInt32(spResponse.ResponseData));
                    }
                    UserModel touser = new UserModel();
                    int sendersid = 0;
                    if (String.IsNullOrEmpty(documentInfo.SenderId) || documentInfo.SenderId == "0")
                    {
                        sendersid = documentInfo.LastCommentedUserID;
                    }
                    else
                    {
                        sendersid = Convert.ToInt32(documentInfo.SenderId);
                    }
                    var userresponse = UtilityHelper.getUserById(Convert.ToInt32(sendersid));
                    var tousers = (List<UserModel>)userresponse.ResponseData;
                    if (tousers.Count > 0)
                    {
                        touser = tousers.FirstOrDefault();
                    }


                    EmailResponse emailResponse = await new WSClientHelper().SendEmail(new EmailRequest()

                    {
                        FromEmail = FromEmail,
                        displayFromEmail = DisplayFromEmail,
                        ToEmail = touser.Email,
                        Subject = RejectedSubject,
                        DocumentName = documentInfo.DocumentName,
                        UserName = touser.RealName,
                        ApproverName = sessionData.RealName,
                        EmailType = EmailType.Reject


                    }, "Rejectapproval", new ServiceParameters { EmailAPIProxyHost = ConfigurationModel.EmailAPIProxyHost, EmailAPIProxyPswd = ConfigurationModel.EmailAPIProxyPswd, isProxyEnableForEmailAPI = ConfigurationModel.isProxyEnableForEmailAPI, EmailURL = ConfigurationModel.EmailURL });
                    AuditHelper.InsertAuditLog("Document", sessionData != null ? sessionData.UserName : "", $"Document (doc id - {documentInfo.DocumentHeaderID})Reject approvalby the user {documentInfo.UserID}");
                }
                else
                {
                    LogHelper.Instance.Debug($"Failled to insert details for approval with reason ({spResponse.ResponseMessage} , code- {spResponse.ResultCode})", "RejectApproval", "Portal", "RejectApproval");
                    //return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, Message = spResponse.ResponseMessage, IsApproval = documentInfo.IsApproval });

                    return Json(new { Result = false, Message = "Success", DetailID = documentInfo.DocumentDetailID, DocumentType = documentInfo.DocumentType, IsApproval = documentInfo.IsApproval });

                }
                //return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, IsApproval = documentInfo.IsApproval });

                return Json(new { Result = true, Message = "Success", DetailID = documentInfo.DocumentDetailID, DocumentType = documentInfo.DocumentType, IsApproval = documentInfo.IsApproval });
            }
            else
            {
                LogHelper.Instance.Debug($"Failled to insert details for approval because document headerid is missing", "Save Document Signature", "Portal", "Document");
                //return RedirectToAction("DocumentDetails", new { DetailID = documentInfo.DocumentDetailID, UserID = documentInfo.UserID, DocumentType = documentInfo.DocumentType, IsPageReloaded = true, Message = "Failled to assign the document for approval", IsApproval = documentInfo.IsApproval });

                return Json(new { Result = false, Message = "Success", DetailID = documentInfo.DocumentDetailID, DocumentType = documentInfo.DocumentType, IsApproval = documentInfo.IsApproval });
            }

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

        public async Task<ActionResult> Upload(DocumentRequestModel docreq)
        {

            string message = ""; int i = 0; int success = 0;
            try
            {

                LogHelper.Instance.Debug($"Upload Started", "Upload", "Portal", "Upload");
                if (Request.Files.Count > 0)
                {
                    foreach (string requestFileName in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[i];
                        UserFormModel usermodel = new UserFormModel();

                        if (file != null && file.ContentLength > 0)
                        {

                            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                            if (extension == ".pdf")
                            {
                                LogHelper.Instance.Debug($"FileType is PDF for filename" + file.FileName + "Position:" + i, "Upload", "Portal", "Upload");

                                var sessionData = (SessionDataModel)Session["GuestDigiDocData"];
                                MemoryStream target = new MemoryStream();
                                file.InputStream.CopyTo(target);
                                byte[] data = target.ToArray();
                                DocumentRequestModel model = new DocumentRequestModel()
                                {
                                    DocumentName = docreq.DocumentName,
                                    DocumentType = docreq.DocumentType,
                                    Username = sessionData.UserName,
                                    DocumentBase64 = Convert.ToBase64String(data),
                                    RoomNo = docreq.RoomNo

                                };

                                var result = UtilityHelper.ProcessDocument(model);
                                if (result.result)
                                {
                                    success++;
                                    string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
                                    LogHelper.Instance.Debug($"Document created for filename" + file.FileName + "Position:" + i, "Upload", "Portal", "Upload");

                                    BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


                                    var res = await new BlobStorage().UploadFileBlobAsync(data, "document" + result.ResponseData + ".pdf", blobServiceClient);
                                    // return Json(new { Result = true, Message = "Document Saved Successfully", Success = true });
                                    LogHelper.Instance.Debug($"Document saved to cloud for filename" + file.FileName + "Position:" + i, "Upload", "Portal", "Upload");
                                }
                                else
                                {
                                    message += file.FileName;
                                    LogHelper.Instance.Debug($"Process Document result is false for file name" + file.FileName + "Position:" + i, "Upload", "Portal", "Upload");
                                }

                            }
                            else
                            {
                                message += file.FileName;
                                LogHelper.Instance.Debug($"Upload a valid .PDF file" + file.FileName + "Position:" + i, "Upload", "Portal", "Upload");
                                //  return Json(new { Result = false, Message = "Upload a valid .PDF file", Success = false });
                            }

                        }
                        else
                        {

                            LogHelper.Instance.Debug($"Process Document file is null for file name" + "Position:" + i, "Upload", "Portal", "Upload");
                            // return Json(new { Result = false, Message = "Upload a valid .PDF file", Success = false });

                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Exception" + ex, "Upload", "Portal", "Upload");

            }
            if (message != "")
            {
                return Json(new { Result = true, Message = message, Success = true });

            }
            else
            {
                if (i == 1 && success == 0)
                {
                    return Json(new { Result = false, Message = "Upload a valid .PDF file", Success = false });
                }
                else
                {

                    return Json(new { Result = true, Message = "Document Saved Successsfully", Success = true });
                }
            }


        }

    }
}