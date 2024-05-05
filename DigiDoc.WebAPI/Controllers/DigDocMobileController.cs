using Azure.Storage.Blobs;
using DigiAPI.Models;
using DigiDoc.DataAccess;
using DigiDoc.Helper;
using DigiDoc.WebAPI.Helper;
using DigiDoc.WebAPI.Models;
using DigiDoc.WebAPI.Models.DigiDocMobile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace DigiDoc.WebAPI.Controllers
{
    public class DigDocMobileController : ApiController
    {
        [HttpGet]
        public async Task<EregResponseModel> GetAllApp()
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all app details", "GetAllApp", "PortalAPI", "GetAllApp");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<AppDetails>("usp_GetAppListt", connectionstring);
                if (result != null)
                {
                    if (result.FirstOrDefault().Result == "200")
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No AppDetails Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetAllApp", "PortalAPI", "GetAllApp");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpGet]
        public async Task<EregResponseModel> GetImages()
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<SliderImage>("usp_GetImages", ConfigurationModel.ConnectionString).ToList();
                if (spResponse != null && spResponse.Count > 0)
                {
                    if (spResponse.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            responseData = spResponse,
                            result = true,
                            statusCode = 200
                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            responseMessage = "Not Found",
                            result = false,
                            statusCode = 201
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP GetImages returned null", "Get Images List", "DigiDocMobile", "GetImages");
                    return new EregResponseModel()
                    {
                        result = false,
                        responseMessage = "DB Error",
                        statusCode = -2
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetImages", "DigiDocMobile", "GetImages");
                return new EregResponseModel()
                {
                    result = false,
                    responseMessage = "Generic exception",
                    statusCode = -1
                };
            }
        }

        [HttpPost]
        public async Task<EregResponseModel> GetAppById(AppDetails appDetails)
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all app details By Id", "GetAppById", "PortalAPI", "GetAppById");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<AppDetails>("usp_GetAppbyId", connectionstring, new { AppId = appDetails.AppId });
                if (result != null)
                {
                    if (result.FirstOrDefault().Result == "200")
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No AppDetails Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetAppById", "PortalAPI", "GetAppById");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }

        [HttpPost]
        public async Task<EregResponseModel> PostTempReservation(TempReservation temp)
        {
            try
            {

                LogHelper.Instance.Debug($"Insert the Resevation Details" + temp.ReservationNumber, "PostTempReservation", "PortalAPI", "PostTempReservation");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
              
              
                var result = dapper.ExecuteSP<EregTempResponseModel>("Usp_InsertUpdateReservationDetails", connectionstring, new
                {
                    ChildCount = temp.ChildCount,
                    DocumentType = temp.DocumentType,
                    ReservationNumber = temp.ReservationNumber,
                    AdultCount = temp.AdultCount,
                    AppId = temp.AppId,
                    ArrivalDate = temp.ArrivalDate,
                    City = temp.City,
                    CountryMasterID = temp.CountryMasterID,
                    FlightNo = temp.FlightNo,
                    DepartureDate = temp.DepartureDate,
                    EmailAddress = temp.EmailAddress,
                    DocumentBase64 = temp.FolioTemplate,
                    PostalCode = temp.PostalCode,
                    RoomType = temp.RoomType,
                    StateMasterID = temp.StateMasterID,
                    OPeraScreenId = temp.ClientConnection,
                    Phone = temp.Phone,
                    AddressLine1 = temp.AddressLine1,
                    MembershipNo = temp.MembershipNo,
                    ProfileId = temp.ProfileId,
                    IsBreakFastAvailable = temp.IsBreakFastAvailable,
                    RoomNo = temp.RoomNo,
                    ReservationNameId = temp.ReservationNameId,
                    FirstName = temp.FirstName,
                    MiddleName = temp.MiddleName,
                    LastName = temp.LastName,
                    AverageRoomRate = temp.AverageRoomRate!=null? temp.AverageRoomRate.Value.ToString("0.00"):"0.00",
                    IsPDf = temp.IsPDf,
                    IsPrintRate = temp.IsPrintRate==null?false:temp.IsPrintRate.Value,
                    
                    
                });


                LogHelper.Instance.Debug($"Reservation inserted successfully" + temp.ReservationNumber, "PostTempReservation", "PortalAPI", "PostTempReservation");

                return new EregResponseModel()
                {
                    result = true,
                    statusCode = 200,
                    responseMessage = $"success",
                    responseData = result.FirstOrDefault().TempID


                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "PostTempReservation", "PortalAPI", "PostTempReservation");
                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
        }
        [HttpPost]
        public async Task<ServiceResponseModel> ProcessDocuments(Document documentRequest)
        {
            try
            {

                string connectionstring = ConfigurationModel.ConnectionString;

                LogHelper.Instance.Debug($"ProcessDocuments started", "ProcessDocuments", "PortalAPI", "ProcessDocuments");
                string connectionstrings = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();

                LogHelper.Instance.Debug($"Update document details" + documentRequest.DocumentId, "ProcessDocument", "PortalAPI", "ProcessDocument");
                byte[] documentBytes = Convert.FromBase64String(documentRequest.Base64Folio);

                var result = dapper.ExecuteSP<DocumentResponseModel>("usp_UpdateDocumentFileDetails", connectionstrings, new
                {
                    DocID = documentRequest.DocumentId,
                    DocumentFile = documentBytes

                });



                string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);

                    await new BlobStorage().DeleteFile(blobServiceClient, "document" + result.FirstOrDefault().DocumentDetailID + ".pdf");

                    await new BlobStorage().UploadFileBlobAsync(documentBytes, "document" + result.FirstOrDefault().DocumentDetailID + ".pdf", blobServiceClient);

                    LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.DocumentId, "ProcessDocument", "PortalAPI", "ProcessDocument");
                }
                var spResponse =
                                new DapperHelper().ExecuteSP<EregResponseModel>("Usp_DeleteTempReservation", ConfigurationModel.ConnectionString,
                                    new { Id = documentRequest.TempId });
                LogHelper.Instance.Debug($"document deleted successfully to blob" + documentRequest.TempId, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new ServiceResponseModel()
                {
                    Result = true,
                    ResponseCode = "200",
                    ResponseMessage = $"success",

                };

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "ProcessDocument", "PortalAPI", "ProcessDocument");
                return new ServiceResponseModel()
                {
                    Result = false,
                    ResponseCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }
        [HttpPost]
        public async Task<EregResponseModel> GetTempReservation(TempReservation req)
        {
            try
            {
                LogHelper.Instance.Debug($"Getting Reservation", "GetTempReservation", "PortalAPI", "GetTempReservation");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<TempReservation>("Usp_GetReservationDetails", connectionstring, new { Id = req.TempId });
                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No Reservation Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetTempReservation", "PortalAPI", "GetTempReservation");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpGet]
        public async Task<EregResponseModel> GetCountryList()
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all country details", "GetCountryList", "PortalAPI", "GetCountryList");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = new DapperHelper().ExecuteSP<CountryMaster>("Usp_GetCountry", ConfigurationModel.ConnectionString);
                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No country Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetCountryList", "PortalAPI", "GetCountryList");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpPost]
        public async Task<EregResponseModel> GetStatesList(Request Id)
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all state", "GetStatesList", "PortalAPI", "GetStatesList");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = new DapperHelper().ExecuteSP<StateMaster>("Usp_GetStates", ConfigurationModel.ConnectionString, new { Id = Id.Id });
                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No AppDetails Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetStatesList", "PortalAPI", "GetStatesList");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpPost]
        public async Task<EregResponseModel> GetPDF(DocumentReq Id)
        {
            try
            {
                LogHelper.Instance.Debug($"Getting PDF from Reservation", "GetPDF", "PortalAPI", "GetPDF");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<DocumentModel>("Usp_GetDocumentfile", connectionstring, new { @DocumentDetailID = Id.Id });
                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No Reservation Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetPDF", "PortalAPI", "GetPDF");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpPost]
        public async Task<EregResponseModel> FetchStatusByCode(NotificationStatus StatusCode)
        {
            try
            {
                var result = new DapperHelper().ExecuteSP<NotificationStatus>("Usp_GetStatus", ConfigurationModel.ConnectionString, new { StatusCode = StatusCode.StatusCode });

                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result.FirstOrDefault().Message

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No AppDetails Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "FetchStatusByCode", "PortalAPI", "FetchStatusByCode");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpPost]
        public async Task<ServiceResponseModel> DeleteDocuments(Document documentRequest)
        {
            try
            {

                string connectionstring = ConfigurationModel.ConnectionString;



                var spResponse =
                                new DapperHelper().ExecuteSP<EregResponseModel>("Usp_DeleteTempReservation", ConfigurationModel.ConnectionString,
                                    new { Id = documentRequest.TempId });
                LogHelper.Instance.Debug($"document deleted successfully" + documentRequest.TempId, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new ServiceResponseModel()
                {
                    Result = true,
                    ResponseCode = "200",
                    ResponseMessage = $"success",

                };

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "DeleteDocuments", "PortalAPI", "DeleteDocuments");
                return new ServiceResponseModel()
                {
                    Result = false,
                    ResponseCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }
        [HttpPost]
        public async Task<EregResponseModel> GetDeviceById(DeviceDetails Details)
        {
            try
            {
                LogHelper.Instance.Debug($"Getting device linked", "GetDeviceById", "PortalAPI", "GetDeviceById");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<DeviceDetails>("Usp_GetDeviceappMapped", connectionstring, new { SystemIP = Details.SystemIP });
                if (result != null)
                {
                    if (result.Count()>0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result.FirstOrDefault()

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No GetDeviceById Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetDeviceById", "PortalAPI", "GetDeviceById");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }

        [HttpGet]
        public async Task<EregResponseModel> GetAllDocuments()
        
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all documents ById", "GetAllDocuments", "PortalAPI", "GetAllDocuments");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<DocumentTypeMaster>("usp_GetAllDocumentType", connectionstring);
                if (result != null)
                {
                    if (result.FirstOrDefault().Result == "200")
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No Document Exists"

                        };
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "GetAllDocuments", "PortalAPI", "GetAllDocuments");

                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
            return new EregResponseModel()
            {
                result = false,
                statusCode = -1,
                responseMessage = null
            };
        }
        [HttpPost]
        [ActionName("FetchGeneralSettings")]
        public async Task<EregResponseModel> FetchGeneralSettings()
        {
            try
            {
                LogHelper.Instance.Debug($"Getting all GeneralSettings", "FetchGeneralSettings", "PortalAPI", "FetchGeneralSettings");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                var result = dapper.ExecuteSP<GeneralSettingsModel>("usp_GetOWSSettingsList", connectionstring);
                if (result != null)
                {
                    if (result.Count()>0)
                    {
                        return new EregResponseModel()
                        {
                            result = true,
                            statusCode = 200,
                            responseMessage = $"Success",
                            responseData = result

                        };
                    }
                    else
                    {
                        return new EregResponseModel()
                        {
                            result = false,
                            statusCode = -1,
                            responseMessage = $"No GeneralSettings Exists"

                        };
                    }
                }
                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"No GeneralSettings Exists"

                };
            }
            catch (Exception ex)
            {
                return new EregResponseModel()
                {
                    result = false,
                    responseMessage = ex.Message,
                    statusCode = -1
                };
            }

        }
        [HttpPost]
        public async Task<EregResponseModel> PostLinkReservation(RequestModel localDataRequest)
        {
            try
            {
                LogHelper.Instance.Debug("PostLinkReservation request into DB : " + JsonConvert.SerializeObject(localDataRequest), "PostLinkReservation", "API", "PostLinkReservation");

                //System.IO.File.AppendAllText(System.Web.HttpContext.Current.Request.MapPath("~\\Resources\\LocalPush.txt"), localDataRequest.RequestObject.ToString());
                List<LinkReservationRequest> reservations = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LinkReservationRequest>>(localDataRequest.RequestObject.ToString());
               

               
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();


                var result = dapper.ExecuteSP<EregTempResponseModel>("Usp_Insert_LinkReservation", connectionstring, new
                {

                    Linkreservation = new DatatableHelper().ToDataTable(reservations),
                    



                });
                bool ActualResult = false;
                if (result != null)
                {
                   ;
                    if (result.FirstOrDefault().Result == "200")
                    {
                        ActualResult= true;
                        LogHelper.Instance.Debug($"Link Reservation inserted successfully", "PostLinkReservation", "PortalAPI", "PostLinkReservation");
                    }
                    else if (result.FirstOrDefault().Result == "201")
                    {
                        ActualResult= false;
                        LogHelper.Instance.Debug($"Link Reservation Error Occured" + reservations.FirstOrDefault().ReservationNumber, "PostLinkReservation", "PortalAPI", "PostLinkReservation");
                    }
                    else if (result.FirstOrDefault().Result == "210")
                    {
                        ActualResult = true;
                        LogHelper.Instance.Debug($"Link Reservation already" + reservations.FirstOrDefault().ReservationNumber, "PostLinkReservation", "PortalAPI", "PostLinkReservation");
                    }
                }

                return new EregResponseModel()
                {
                    result = ActualResult,
                    statusCode = 200,
                    responseMessage = result!=null?result.FirstOrDefault().Message:"success",
                    responseData = null


                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "PostTempReservation", "PortalAPI", "PostTempReservation");
                return new EregResponseModel()
                {
                    result = false,
                    statusCode = -1,
                    responseMessage = $"Generic error : {ex.Message}"
                };
            }
        }



    }


}