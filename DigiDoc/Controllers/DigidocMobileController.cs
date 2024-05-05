
using Azure.Storage.Blobs;
using DigiDoc.DataAccess;
using DigiDoc.Helper;
using DigiDoc.Models;
using DigiDoc.Models.Ereg;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DigiDoc.Controllers.DigiDocMobile
{
    public class DigidocMobileController : Controller
    {
       
        /// <summary>
        /// Get the Slider not using
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SliderIndex(string AppId)
        {

            LogHelper.Instance.Debug($"Digital Document started loading images", "SliderIndex", "Portal", "SliderIndex");

            List<SliderImage> imgdetails = null;
            ViewBag.AppId = AppId;
            ViewBag.IsAssigned="0";
           
            ViewBag.DefaultTime = ConfigurationManager.AppSettings["DefaultTime"].ToString();
            ViewBag.LoadTime = ConfigurationManager.AppSettings["LoadSignalRTime"].ToString();
            ViewBag.IsPageReload = ConfigurationManager.AppSettings["IsPageReload"].ToString();

            
            return View(imgdetails);
        }
        /// <summary>
        /// Get the views
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetView(string Id)
        {
            LogHelper.Instance.Debug($"Digital Document get the corresponding view  given Id is started", "GetView", "Portal", "GetView");

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var foliodetails = DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(Id));
                   

                    if (foliodetails!=null)
                    {
                        LogHelper.Instance.Debug($"Digital Document get the details using the given Id", "GetView", "Portal", "GetView");
                        if (foliodetails.result)
                        {
                            DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;
                            byte[] folio = tempReservation.FolioTemplate;
                            if (tempReservation.IsPDf)
                                {
                                    var spResponse = DigiDocMobileHelper.UpdateAppDetails(new Models.Ereg.AppDetails()
                                    {
                                        AppName = tempReservation.AppId,
                                        connectionId = null,
                                        IsAssinged = true,
                                    });
                               
                                if (folio != null)
                                {
                                    System.IO.File.WriteAllBytes(Server.MapPath($"~/temp/{tempReservation.TempReservationID}.pdf"), folio);
                                }
                                return PartialView("~/Views/DigidocMobile/GetAnyPDF.cshtml", tempReservation);

                                }
                                else
                                {
                                if (tempReservation.DocumentType == ConfigurationManager.AppSettings["RegCard"].ToString())
                                {
                                    var CountryList = DigiDocMobileHelper.GetCountryList();
                                    if (CountryList != null)
                                    {
                                        List<CountryMaster> country = (List<CountryMaster>)CountryList.ResponseData;
                                        if (tempReservation.CountryMasterID != null && tempReservation.CountryMasterID != "0")
                                        {
                                            ViewBag.CountryList = new SelectList(country, "CountryMasterID", "Country_Full_name", Convert.ToInt32(tempReservation.CountryMasterID));

                                        }
                                        else
                                        {
                                            ViewBag.CountryList = new SelectList(country, "CountryMasterID", "Country_Full_name", "Select Country");

                                        }
                                    }
                                    else
                                    {
                                        List<CountryMaster> country = new List<CountryMaster>();
                                        ViewBag.CountryList = new SelectList(country, "CountryMasterID", "Country_Full_name", "Select Country");

                                    }
                                    if (tempReservation.CountryMasterID != null)
                                    {
                                        var StateList = DigiDocMobileHelper.GetStatesList(Convert.ToInt32(tempReservation.CountryMasterID));
                                        List<StateMaster> state = (List<StateMaster>)StateList.ResponseData;

                                        if (tempReservation.StateMasterID != null && tempReservation.StateMasterID != "0" && tempReservation.StateMasterID != "Select State")
                                        {
                                            ViewBag.StateList = new SelectList(state, "StateMasterID", "Statename", Convert.ToInt32(tempReservation.StateMasterID));

                                        }
                                        else
                                        {
                                            ViewBag.StateList = new SelectList(state, "StateMasterID", "Statename", tempReservation.StateMasterID);

                                        }

                                    }
                                    else
                                    {
                                        List<StateMaster> tbstateMasters = new List<StateMaster>();
                                        tbstateMasters.Add(new StateMaster()
                                        {
                                            Statename = "Please select country",
                                            StateMasterID = -1

                                        });
                                        ViewBag.StateList = new SelectList(tbstateMasters, "StateMasterID", "Statename", "Select State");
                                    }
                                    var spResponse = DigiDocMobileHelper.UpdateAppDetails(new Models.Ereg.AppDetails()
                                    {
                                        AppName = tempReservation.AppId,
                                        connectionId = null,
                                        IsAssinged = true,
                                    });
                                    return PartialView("~/Views/DigidocMobile/GetRegistration.cshtml", tempReservation);

                                }
                                else if (tempReservation.DocumentType == ConfigurationManager.AppSettings["Folio"].ToString())
                                {
                                    if (folio != null)
                                    {
                                        System.IO.File.WriteAllBytes(Server.MapPath($"~/temp/{tempReservation.TempReservationID}.pdf"), folio);
                                    }
                                    var spResponse = DigiDocMobileHelper.UpdateAppDetails(new Models.Ereg.AppDetails()
                                    {
                                        AppName = tempReservation.AppId,
                                        connectionId = null,
                                        IsAssinged = true,
                                    });
                                    return PartialView("~/Views/DigidocMobile/GetFolio.cshtml", tempReservation);
                                }
                                else
                                {
                                    return View("SliderIndex");
                                }
                            }

                            

                        }
                        else
                        {
                            LogHelper.Instance.Warn($"Digital Document get the registration result is false", "GetView", "Portal", "GetView");
                            return null;

                        }
                    }
                else
                {
                    LogHelper.Instance.Warn($"Digital Document get the registration given Id is null or empty", "GetView", "Portal", "GetView");

                    return null;

                }
            
                }
                else
                {
                    LogHelper.Instance.Warn($"Digital Document GetView given Id is null or empty", "GetView", "Portal", "GetView");

                    return null;

                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }
      /// <summary>
      /// Get the document to pdf
      /// </summary>
      /// <param name="Id"></param>
      /// <returns></returns>
        public async Task<FileResult> GetPDFStream(int Id)
        {

            try
            {
                LogHelper.Instance.Log("Get Folio Details started", "GetPDFStream", "Portal", "GetPDFStream");

                var foliodetails =  DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(Id));
                if (foliodetails != null)
                {
                    if (foliodetails.result)
                    {
                        DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;
                        MemoryStream Stream = new MemoryStream(tempReservation.FolioTemplate);
                        Stream.Write(tempReservation.FolioTemplate, 0, tempReservation.FolioTemplate.Length);
                        Stream.Position = 0;
                        Response.AddHeader("Content-Disposition", "inline; filename=" + tempReservation.ReservationNumber + ".PDF");
                        return File(Stream, "application/pdf");
                    }
                    else
                    {
                        LogHelper.Instance.Log("Get Folio Details result is false", "GetPDFStream", "Portal", "GetPDFStream");

                    }
                }
                else
                {
                    LogHelper.Instance.Log("Get Folio Details is null", "GetPDFStream", "Portal", "GetPDFStream");
                }
               
            }
            catch(Exception ex)
            {
                LogHelper.Instance.Error(ex.InnerException, "GetPDFStream", "Portal", "GetPDFStream");

            } 
            return null;
        }

      

        [HttpPost]
        public async Task<ActionResult> RejectReservation(int Id)
        {
            try
            {
                LogHelper.Instance.Debug("Reject Reservation Started", "RejectReservation", "Portal", "RejectReservation");

                var foliodetails = DigiDocMobileHelper.RejectReservation(Convert.ToInt32(Id));
                return Json(new { Result = foliodetails.result, Message = foliodetails.ResponseMessage });
            }
            catch (Exception ex)
            { 
                LogHelper.Instance.Error(ex.InnerException, "RejectReservation", "Portal", "RejectReservation");  
                return Json(new { Result = false, Message = ex.Message });
              
            }
           

        }

        [HttpPost]
        
        public async Task<ActionResult> SaveFolioSignature(TempReservationRequests id)
        {
            try
            {
                LogHelper.Instance.Debug("SaveFolioSignature Started" + id.TempReservationID, "SaveFolioSignature", "Portal", "SaveFolioSignature");

                var IsMCIEnabled = ConfigurationManager.AppSettings["IsMCIEnabled"];
                if(IsMCIEnabled==null || IsMCIEnabled=="")
                {
                    IsMCIEnabled = "false";
                }
                TempReservationRequest temp = new TempReservationRequest();
                temp.Base64Signature = id.Base64Signature;
                temp.DocumentType = id.DocumentType;
                temp.TempReservationID = id.TempReservationID;
                temp.Phone = id.Phone;
                 temp.EmailAddress = id.EmailAddress;
                if (temp != null)
                {
                  
                        try
                        {
                            var foliodetails = DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(temp.TempReservationID));

                            DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;
                        temp.EmailAddress = tempReservation.EmailAddress;
                        LogHelper.Instance.Debug($"document starting processing named" + temp.TempReservationID, "SaveFolioSignature", "PortalAPI", "SaveFolioSignature");
                            string connectionstring = ConfigurationModel.ConnectionString;
                            DapperHelper dapper = new DapperHelper();

                            string PDFBase64 = Convert.ToBase64String(tempReservation.FolioTemplate);
                            string name = "sfds";
                            var timezoneid = "Singapore Standard Time";
                        temp.FolioTemplate = tempReservation.FolioTemplate;
                            DateTime propertytimezone = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
                        //var xmlpath = ConfigurationManager.AppSettings["XMLPATH"];

                        //DataSet dsMenu = new DataSet(); //Create Dataset Object

                        //dsMenu.ReadXml(xmlpath); // Read XML file in Dataset

                        //DataTable dtXMLFILE = new DataTable();// Create DatyaTable object
                        //string signaturetitle = null;
                        //dtXMLFILE = dsMenu.Tables[0];
                        //if (dtXMLFILE != null)
                        //{
                        //    signaturetitle = (from DataRow dr in dtXMLFILE.Rows
                        //                where (string)dr["DocumentType"] == tempReservation.DocumentType.ToString()
                        //                select (string)dr["Signature"]).FirstOrDefault();
                        //}
                        //    if (temp.DocumentType != "RegcardTemplate" && temp.DocumentType != "FolioTemplate")
                        //    {
                        //        PdfHelper.Model.ResponseModel response = PdfHelper.PdfHelper.FolioTemplate(PDFBase64, temp.Base64Signature, 50, 50, name, propertytimezone, "", signaturetitle);
                        //        temp.FolioTemplate = System.Convert.FromBase64String(response.Data.ToString());

                        //    }
                        
                            temp.ReservationNumber= tempReservation.ReservationNumber;
                           var tempupdate = DigiDocMobileHelper.UpdateRegistration(temp);
                        if (!Convert.ToBoolean(IsMCIEnabled))
                        {
                            var existcheck = DigiDocMobileHelper.CheckDocumentExistsDocument(temp);
                            var doc = new DigiMobileResponseModel();
                            if (existcheck != null)
                            {
                                var docdetail = existcheck.ResponseData.ToString();
                                if (string.IsNullOrEmpty(docdetail) || docdetail == "0")
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp);
                                    LogHelper.Instance.Debug($"document inserted successfully to database", "SaveFolioSignature", "Portal", "SaveFolioSignature");
                                }
                                else
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp, docdetail);
                                    //doc = DigiDocMobileHelper.UpdateDocument(Convert.ToInt32(docdetail), temp.FolioTemplate);
                                    LogHelper.Instance.Debug($"document updated to database", "SaveFolioSignature", "Portal", "SaveFolioSignature");
                                }

                            }
                            else
                            {
                                doc = DigiDocMobileHelper.ProcessDocument(temp);
                                LogHelper.Instance.Debug($"document inserted successfully to database", "SaveFolioSignature", "Portal", "SaveFolioSignature");
                            }

                          
                                if (doc.result)
                                {
                                   
                                    string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
                                if (!string.IsNullOrEmpty(ConnectionString))
                                {
                                    BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


                                    await new BlobStorage().UploadFileBlobAsync(temp.FolioTemplate, "document" + doc.ResponseData.ToString() + ".pdf", blobServiceClient);
                                }
                                string FilePath = Server.MapPath($"~/temp/{tempReservation.TempReservationID}.pdf");
                                if (System.IO.File.Exists(FilePath))
                                {
                                    System.IO.File.Delete(FilePath);
                                }
                                LogHelper.Instance.Debug($"document inserted successfully to blob", "SaveFolioSignature", "Portal", "SaveFolioSignature");
                                    return Json(new { Result = true, Message = "Success", DocumentId = doc.ResponseData.ToString(),TempId = temp.TempReservationID });
                                }
                                else
                                {
                                    return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = temp.TempReservationID });
                                }
                            
                        }
                        else
                        {
                            return Json(new { Result = true, Message = "Error", DocumentId = 0, TempId = temp.TempReservationID });

                        }
                    }
                        catch (Exception ex)
                        {
                            LogHelper.Instance.Debug($"Error" + ex, "SaveFolioSignature", "Portal", "SaveFolioSignature");

                        }
                    
                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }
                else
                {
                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }
               

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex.InnerException, "SaveFolioSignature", "Portal", "SaveFolioSignature");

                return Json(new { Result = "Exception", Message = "Exception" });

            }
        }
        [HttpPost]

        public async Task<ActionResult> SaveRegCardSignature(TempReservationRequest2 id)
        {
            try
            {
                LogHelper.Instance.Debug($"document starting processing named" + id.TempReservationID, "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                var IsMCIEnabled = ConfigurationManager.AppSettings["IsMCIEnabled"];
                if (IsMCIEnabled == null || IsMCIEnabled == "")
                {
                    IsMCIEnabled = "false";
                }
                TempReservationRequest temp = new TempReservationRequest();
                temp.Base64Signature = id.Base64Signature;
                temp.DocumentType = id.DocumentType;
                temp.EmailAddress=id.EmailAddress;
                temp.Phone=id.Phone;
                temp.AddressLine1=id.AddressLine1;
                temp.PostalCode=id.PostalCode;
                temp.CountryMasterID=id.CountryMasterID;
                temp.StateMasterID=id.StateMasterID;
                temp.City=id.City;
                temp.TempReservationID = id.TempReservationID;
                if (temp != null)
                {
                  
                        try
                        {
                            var foliodetails = DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(temp.TempReservationID));
                            DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;
                          
                            temp.FolioTemplate = null;
                            temp.ReservationNumber = tempReservation.ReservationNumber;
                            temp.RoomNo = tempReservation.RoomNo;
                            temp.ArrivalDate = tempReservation.ArrivalDate;
                            temp.DepartureDate = tempReservation.DepartureDate;
                            temp.GuestName = tempReservation.FirstName;
                            temp.CountryName = tempReservation.CountryName;
                            temp.StateName = tempReservation.StateName;
                        var tempupdate = DigiDocMobileHelper.UpdateRegistration(temp);
                        if (!Convert.ToBoolean(IsMCIEnabled))
                        {
                            var existcheck = DigiDocMobileHelper.CheckDocumentExistsDocument(temp);
                            var doc = new DigiMobileResponseModel();
                            if (existcheck != null)
                            {
                                var docdetail = existcheck.ResponseData.ToString();
                                if (string.IsNullOrEmpty(docdetail) || docdetail == "0")
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp);
                                }
                                else
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp,docdetail);
                                }
                            }
                            else
                            {
                                doc = DigiDocMobileHelper.ProcessDocument(temp);
                            }


                            if (doc.result)
                            {
                                LogHelper.Instance.Debug($"document inserted successfully to blob", "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                                return Json(new { Result = true, Message = "Success", DocumentId = doc.ResponseData.ToString(), TempId = tempReservation.TempReservationID, ClientConnection = tempReservation.ClientConnection });

                            }
                            else
                            {
                                return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = 0, ClientConnection = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { Result = true, Message = "Success", DocumentId = 0, TempId = tempReservation.TempReservationID, ClientConnection = tempReservation.ClientConnection });

                        }

                    }
                        catch (Exception ex)
                        {
                            LogHelper.Instance.Debug($"Error" + ex, "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                            return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = 0, ClientConnection = 0 });
                        }

                   

                }
                else
                {
                    LogHelper.Instance.Debug($"Reservation details is empty", "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }
              

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug(ex.Message, "SaveRegCardSignature", "Portal", "SaveRegCardSignature");

                return Json(new { Result = "Exception", Message = "Exception" });

            }
        }
        public async Task<JsonResult> State_Bind(string country_id)
        {
            try
            {
                var StateList = DigiDocMobileHelper.GetStatesList(Convert.ToInt32(country_id));
                List<StateMaster> State = (List<StateMaster>)StateList.ResponseData;
                return Json(State, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                LogHelper.Instance.Debug(ex.Message, "State_Bind", "Portal", "State_Bind");

                List<StateMaster> State = new List<StateMaster>();
                return Json(State, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]

        public async Task<ActionResult> SaveAnyPDfSignature(TempReservationRequests id)
        {
            try
            {
                LogHelper.Instance.Debug("SaveFolioSignature Started" + id.TempReservationID, "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");

                var IsMCIEnabled = ConfigurationManager.AppSettings["IsMCIEnabled"];
                if (IsMCIEnabled == null || IsMCIEnabled == "")
                {
                    IsMCIEnabled = "false";
                }
                TempReservationRequest temp = new TempReservationRequest();
                temp.Base64Signature = id.Base64Signature;
                temp.DocumentType = id.DocumentType;
                temp.TempReservationID = id.TempReservationID;
                temp.Phone = id.Phone;
                temp.EmailAddress = id.EmailAddress;
                
                if (temp != null)
                {

                    try
                    {
                        var foliodetails = DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(temp.TempReservationID));

                        DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;
                        temp.EmailAddress = tempReservation.EmailAddress;
                        temp.Phone = tempReservation.Phone;
                        temp.RoomNo = tempReservation.RoomNo;
                        LogHelper.Instance.Debug($"document starting processing named" + temp.TempReservationID, "SaveAnyPDfSignature", "PortalAPI", "SaveAnyPDfSignature");
                        string connectionstring = ConfigurationModel.ConnectionString;
                        DapperHelper dapper = new DapperHelper();

                        string PDFBase64 = Convert.ToBase64String(tempReservation.FolioTemplate);
                        string name = "sfds";
                        var timezoneid = "Singapore Standard Time";
                        List<DocumentPdfModel> pdfModels = new List<DocumentPdfModel>();
                        DateTime propertytimezone = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
                        var xmlpath = ConfigurationManager.AppSettings["XMLPATH"];

                        using (var xmlReader = new StreamReader(xmlpath))
                        {
                            XDocument docxml = XDocument.Load(xmlReader);
                            var emplist = docxml.Descendants("PDFTYPES").Elements("PDFTYPE").
                               Select(x => new DocumentPdfModel()
                               {
                                   DocumentType = x.Element("DocumentType").Value,
                                   imgheight = x.Element("imgheight").Value,
                                   imgwidth = x.Element("imgwidth").Value,
                                   imgxaxis = x.Element("imgxaxis").Value,
                                   imgyaxis = x.Element("imgyaxis").Value,
                                   Signature = x.Element("Signature").Value

                               }).ToList();
                            pdfModels = emplist;
                        }

                        if (pdfModels.Count > 0)
                        {
                            pdfModels = pdfModels.Where(x => x.DocumentType == tempReservation.DocumentType.ToString()).ToList();

                        }
                        if (pdfModels.Count <= 0)
                        {
                            pdfModels.Add(new DocumentPdfModel()
                            {
                                DocumentType = ConfigurationManager.AppSettings["OtherDocument"],
                                imgheight = "100",
                                imgwidth = "100",
                                imgxaxis = "10",
                                imgyaxis = "10",
                                Signature = null
                            });

                        }

                       
                        PdfHelper.Model.ResponseModel response = PdfHelper.PdfHelper.FolioTemplate(PDFBase64, temp.Base64Signature, Convert.ToInt32(pdfModels.FirstOrDefault().imgwidth), Convert.ToInt32(pdfModels.FirstOrDefault().imgheight), name, propertytimezone, "", pdfModels.FirstOrDefault().Signature, pdfModels.FirstOrDefault().imgxaxis, pdfModels.FirstOrDefault().imgyaxis);
                        temp.FolioTemplate = System.Convert.FromBase64String(response.Data.ToString());


                        temp.ReservationNumber = tempReservation.ReservationNumber;
                        var tempupdate = DigiDocMobileHelper.UpdateRegistration(temp);
                        if (!Convert.ToBoolean(IsMCIEnabled))
                        {
                            var existcheck = DigiDocMobileHelper.CheckDocumentExistsDocument(temp);

                            var doc = new DigiMobileResponseModel();
                            if (existcheck != null)
                            {
                                var docdetail = existcheck.ResponseData.ToString();
                                if (string.IsNullOrEmpty(docdetail) || docdetail == "0")
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp);
                                    LogHelper.Instance.Debug($"document inserted successfully to database", "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");
                                }
                                else
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp, docdetail);
                                    // doc = DigiDocMobileHelper.UpdateDocument(Convert.ToInt32(docdetail), temp.FolioTemplate);
                                    LogHelper.Instance.Debug($"document updated to database", "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");
                                }

                            }
                            else
                            {
                                doc = DigiDocMobileHelper.ProcessDocument(temp);
                                LogHelper.Instance.Debug($"document inserted successfully to database", "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");
                            }


                            if (doc.result)
                            {

                                string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
                                if (!string.IsNullOrEmpty(ConnectionString))
                                {
                                    BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);


                                    await new BlobStorage().UploadFileBlobAsync(temp.FolioTemplate, "document" + doc.ResponseData.ToString() + ".pdf", blobServiceClient);
                                }
                                string FilePath = Server.MapPath($"~/temp/{tempReservation.TempReservationID}.pdf");
                                if (System.IO.File.Exists(FilePath))
                                {
                                    System.IO.File.Delete(FilePath);
                                }

                                LogHelper.Instance.Debug($"document inserted successfully to blob", "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");
                                return Json(new { Result = true, Message = "Success", DocumentId = doc.ResponseData.ToString(), TempId = temp.TempReservationID });
                            }
                            else
                            {
                                return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = temp.TempReservationID });
                            }

                        }
                        else
                        {
                            return Json(new { Result = true, Message = "Success", DocumentId = 0, TempId = temp.TempReservationID });

                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Debug($"Error" + ex, "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");

                    }

                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }
                else
                {
                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }


            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex.InnerException, "SaveAnyPDfSignature", "Portal", "SaveAnyPDfSignature");

                return Json(new { Result = "Exception", Message = "Exception" });

            }
        }
        [HttpPost]
        public async Task<ActionResult> SaveRegCardSignatureWOI(TempReservationRequest2 id)
        {
            try
            {
                LogHelper.Instance.Debug($"document starting processing named" + id.TempReservationID, "SaveRegCardSignatureWithOutOpera", "Portal", "SaveRegCardSignatureWithOutOpera");
                var IsMCIEnabled = ConfigurationManager.AppSettings["IsMCIEnabled"];
                if (IsMCIEnabled == null || IsMCIEnabled == "")
                {
                    IsMCIEnabled = "false";
                }
                TempReservationRequest temp = new TempReservationRequest();
                temp.Base64Signature = id.Base64Signature;
                temp.DocumentType = id.DocumentType;
                temp.EmailAddress = id.EmailAddress;
                temp.Phone = id.Phone;
                temp.AddressLine1 = id.AddressLine1;
                temp.PostalCode = id.PostalCode;
                temp.CountryMasterID = id.CountryMasterID;
                temp.StateMasterID = id.StateMasterID;
                temp.City = id.City;
                temp.TempReservationID = id.TempReservationID;
                if (temp != null)
                {

                    try
                    {
                        var foliodetails = DigiDocMobileHelper.GetReservationDetails(Convert.ToInt32(temp.TempReservationID));
                        DigiDoc.Models.Ereg.TempReservation tempReservation = (DigiDoc.Models.Ereg.TempReservation)foliodetails.ResponseData;

                        temp.FolioTemplate = tempReservation.FolioTemplate;
                        temp.ReservationNumber = tempReservation.ReservationNumber;
                        temp.RoomNo= tempReservation.RoomNo;
                        temp.ArrivalDate=tempReservation.ArrivalDate;
                        temp.DepartureDate=tempReservation.DepartureDate;
                        temp.GuestName = tempReservation.FirstName;
                        temp.CountryName = tempReservation.CountryName;
                        temp.StateName = tempReservation.StateName;
                        #region UpdatePDF

                        string PDFBase64 = Convert.ToBase64String(tempReservation.FolioTemplate);
                        string name = "sfds";
                        var timezoneid = "Singapore Standard Time";
                        List<PdfHelper.Model.DocumentPdfModel> pdfModels = new List<PdfHelper.Model.DocumentPdfModel>();
                        DateTime propertytimezone = DateTimeHelper.ConvertFromUTC(DateTime.UtcNow, timezoneid);
                        var xmlpath = ConfigurationManager.AppSettings["XMLPATH"];

                        using (var xmlReader = new StreamReader(xmlpath))
                        {
                            XDocument docxml = XDocument.Load(xmlReader);
                            var emplistelement = docxml.Descendants("PDFTYPES").Elements("PDFTYPE");
                           
                            var emplist = docxml.Descendants("PDFTYPES").Elements("PDFTYPE").
                               Select(x => new PdfHelper.Model.DocumentPdfModel()
                               {
                                   DocumentType = x.Element("DocumentType")!=null?x.Element("DocumentType").Value:"",
                                   imgheight = x.Element("imgheight")!=null? x.Element("imgheight").Value:"",
                                   imgwidth = x.Element("imgheight")!=null?x.Element("imgwidth").Value:"",
                                   imgxaxis = x.Element("imgxaxis")!=null?x.Element("imgxaxis").Value:"",
                                   imgyaxis = x.Element("imgyaxis")!=null?x.Element("imgyaxis").Value:"",
                                   Signature = x.Element("Signature")!=null?x.Element("Signature").Value:"",
                                   Address= x.Element("Address")!=null?x.Element("Address").Value:"",
                                   Addressxaxis= x.Element("Addressxaxis")!=null?x.Element("Addressxaxis").Value:"",
                                   City= x.Element("City")!=null?x.Element("City").Value:"",
                                   Cityxaxis= x.Element("Cityxaxis")!=null?x.Element("Cityxaxis").Value:"",

                                   Cityyaxis= x.Element("Cityyaxis") != null?x.Element("Cityyaxis").Value:"",
                                   Country= x.Element("Country") != null ? x.Element("Country").Value:"",
                                   Countryxaxis= x.Element("Countryxaxis") != null ? x.Element("Countryxaxis").Value:"",
                                   Countryyaxis= x.Element("Countryyaxis") != null ? x.Element("Countryyaxis").Value:"",
                                   Email= x.Element("Email") != null ? x.Element("Email").Value:"",
                                   EmailAddressxaxis= x.Element("EmailAddressxaxis") != null ? x.Element("EmailAddressxaxis").Value:"",
                                   EmailAddressyaxis= x.Element("EmailAddressyaxis") != null ? x.Element("EmailAddressyaxis").Value:"",
                                   Phone= x.Element("Phone") != null ? x.Element("Phone").Value:"",
                                   PhoneNumberxaxis= x.Element("PhoneNumberxaxis") != null ? x.Element("PhoneNumberxaxis").Value:"",
                                   PhoneNumberyaxis= x.Element("PhoneNumberyaxis") != null ? x.Element("PhoneNumberyaxis").Value:"",
                                   PinCode= x.Element("PinCode") != null ? x.Element("PinCode").Value:"",
                                   PinCodexaxis= x.Element("PinCodexaxis") != null ? x.Element("PinCodexaxis").Value:"",
                                   PinCodeyaxis= x.Element("PinCodeyaxis") != null ? x.Element("PinCodeyaxis").Value:"",
                                   State = x.Element("State") != null?x.Element("State").Value:"",
                                   Statexaxis= x.Element("Statexaxis") != null?x.Element("Statexaxis").Value:"",
                                   Stateyaxis= x.Element("Stateyaxis") != null ? x.Element("Stateyaxis").Value:"",
                                   Addressyaxis= x.Element("Addressyaxis") != null ? x.Element("Addressyaxis").Value:"",
                     Phonewidth= x.Element("Phonewidth") != null ? x.Element("Phonewidth").Value : "",
        EmailAddresswidth = x.Element("EmailAddresswidth") != null ? x.Element("EmailAddresswidth").Value : "",
                                   Addresswidth = x.Element("Phonewidth") != null ? x.Element("Phonewidth").Value : "",
                                   Countrywidth = x.Element("Countrywidth") != null ? x.Element("Countrywidth").Value : "",
         Statewidth = x.Element("Statewidth") != null ? x.Element("Statewidth").Value : "",
                                   Citywidth = x.Element("Citywidth") != null ? x.Element("Citywidth").Value : "",
                                   PinCodewidth = x.Element("PinCodewidth") != null ? x.Element("PinCodewidth").Value : "",
         Phoneheight = x.Element("Phoneheight") != null ? x.Element("Phoneheight").Value : "",
                                   EmailAddressheight = x.Element("EmailAddressheight") != null ? x.Element("EmailAddressheight").Value : "",
         Addressheight = x.Element("Addressheight") != null ? x.Element("Addressheight").Value : "",
         Countryheight = x.Element("Countryheight") != null ? x.Element("Countryheight").Value : "",
         Stateheight = x.Element("Stateheight") != null ? x.Element("Stateheight").Value : "",
         Cityheight = x.Element("Cityheight") != null ? x.Element("Cityheight").Value : "",
         PinCodeheight = x.Element("PinCodeheight") != null ? x.Element("PinCodeheight").Value : "",
                                   Phonesize = x.Element("Phonesize") != null ? x.Element("Phoneheight").Value : "",
                                   EmailAddresssize = x.Element("EmailAddresssize") != null ? x.Element("EmailAddresssize").Value : "",
                                   Addresssize = x.Element("Addresssize") != null ? x.Element("Addresssize").Value : "",
                                   Countrysize = x.Element("Countrysize") != null ? x.Element("Countrysize").Value : "",
                                   Statesize = x.Element("Statesize") != null ? x.Element("Statesize").Value : "",
                                   Citysize = x.Element("Citysize") != null ? x.Element("Citysize").Value : "",
                                   PinCodesize = x.Element("PinCodesize") != null ? x.Element("PinCodesize").Value : "",
                                   Email2 = x.Element("Email2") != null ? x.Element("Email2").Value : "",
                                   Email2Addressxaxis = x.Element("Email2Addressxaxis") != null ? x.Element("Email2Addressxaxis").Value : "",
                                   Email2Addressyaxis = x.Element("Email2Addressyaxis") != null ? x.Element("Email2Addressyaxis").Value : "",
                                   Email2Addressheight = x.Element("Email2Addressheight") != null ? x.Element("Email2Addressheight").Value : "",
                                   Email2Addresswidth = x.Element("Email2Addresswidth") != null ? x.Element("Email2Addresswidth").Value : "",
                                   Email2Addresssize = x.Element("Email2Addresssize") != null ? x.Element("Email2Addresssize").Value : ""

                               }).ToList();
                            pdfModels = emplist;
                        }

                        if (pdfModels.Count > 0)
                        {
                            pdfModels = pdfModels.Where(x => x.DocumentType == tempReservation.DocumentType.ToString()).ToList();
                            
                        }
                        if (pdfModels.Count <= 0)
                        {
                            pdfModels.Add(new PdfHelper.Model.DocumentPdfModel()
                            {
                                DocumentType = ConfigurationManager.AppSettings["OtherDocument"],
                                imgheight = "100",
                                imgwidth = "100",
                                imgxaxis = "10",
                                imgyaxis = "10",
                                Signature = null
                            });

                        }
                        else
                        {
                           

                            foreach (var t1 in pdfModels.ToList().Select(t => t))
                            {
                                t1.EmailValue = temp.EmailAddress;
                                t1.AddressValue = temp.AddressLine1;
                                t1.PhoneValue = temp.Phone;
                                t1.CityValue = temp.City;
                                t1.PinCodeValue = temp.PostalCode;
                                t1.CountryValue = temp.CountryName;
                                t1.StateValue = temp.StateName;

                            }
                           
                        }

                        
                        PdfHelper.Model.ResponseModel response = PdfHelper.PdfHelper.RegCardTemplate(PDFBase64, temp.Base64Signature, Convert.ToInt32(pdfModels.FirstOrDefault().imgwidth), Convert.ToInt32(pdfModels.FirstOrDefault().imgheight), name, propertytimezone, "", pdfModels.FirstOrDefault().Signature, pdfModels.FirstOrDefault().imgxaxis, pdfModels.FirstOrDefault().imgyaxis, pdfModels);
                        temp.FolioTemplate = System.Convert.FromBase64String(response.Data.ToString());
                        #endregion
                        var tempupdate = DigiDocMobileHelper.UpdateRegistration(temp);
                        if (!Convert.ToBoolean(IsMCIEnabled))
                        {
                            var existcheck = DigiDocMobileHelper.CheckDocumentExistsDocument(temp);
                            var doc = new DigiMobileResponseModel();
                            if (existcheck != null)
                            {
                                var docdetail = existcheck.ResponseData.ToString();
                                if (string.IsNullOrEmpty(docdetail) || docdetail == "0")
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp);
                                }
                                else
                                {
                                    doc = DigiDocMobileHelper.ProcessDocument(temp, docdetail);
                                }
                            }
                            else
                            {
                                doc = DigiDocMobileHelper.ProcessDocument(temp);
                            }


                            if (doc.result)
                            {
                                LogHelper.Instance.Debug($"document inserted successfully to blob", "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                                return Json(new { Result = true, Message = "Success", DocumentId = doc.ResponseData.ToString(), TempId = tempReservation.TempReservationID, ClientConnection = tempReservation.ClientConnection });

                            }
                            else
                            {
                                return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = 0, ClientConnection = 0 });
                            }
                        }
                        else
                        {
                            return Json(new { Result = true, Message = "Success", DocumentId = 0, TempId = tempReservation.TempReservationID, ClientConnection = tempReservation.ClientConnection });

                        }

                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Debug($"Error" + ex, "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                        return Json(new { Result = false, Message = "Error", DocumentId = 0, TempId = 0, ClientConnection = 0 });
                    }



                }
                else
                {
                    LogHelper.Instance.Debug($"Reservation details is empty", "SaveRegCardSignature", "Portal", "SaveRegCardSignature");
                    return Json(new { Result = false, Message = "Exception", DocumentId = 0, TempId = 0 });

                }


            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug(ex.Message, "SaveRegCardSignature", "Portal", "SaveRegCardSignature");

                return Json(new { Result = "Exception", Message = "Exception" });

            }
        }

    }
}