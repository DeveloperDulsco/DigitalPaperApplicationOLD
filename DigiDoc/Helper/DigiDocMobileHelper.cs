using DigiDoc.DataAccess;
using DigiDoc.DataAccess.Models;
using DigiDoc.Models;
using DigiDoc.Models.Ereg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Helper
{
    public class DigiDocMobileHelper
    {


        public static DigiMobileResponseModel UpdateAppDetails(AppDetails app)
        {
            try
            {
                var spResponse = 
                                    new DapperHelper().ExecuteSP<SPResponseModel>("Usp_UpdateAppDetails", ConfigurationModel.ConnectionString,
                                        new { Appname = app.AppName, connectionId =app.connectionId, IsAssinged=app.IsAssinged})
                                   ;
                if (spResponse != null)
                {
                    if (spResponse.First().Result.Equals("200"))
                    {
                        return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    if (spResponse.First().Result.Equals("201"))
                    {
                        return new DigiMobileResponseModel()
                        {
                            result = false,
                            ResultCode = "201",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"SP UpdateAppDetails returned Error Code : {spResponse.First().Result} and Message : {spResponse.First().Message}", "UpdateAppDetails", "Portal", "UpdateAppDetails");
                        return new DigiMobileResponseModel()
                        {
                            result = false,
                            ResultCode = spResponse.First().Result,
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                }
                else
                {
                    LogHelper.Instance.Debug("SP UpdateAppDetails returned null", "DigiMobile", "Portal", "UpdateAppDetails");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "UpdateAppDetails", "DigiMobile", "UpdateAppDetails");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static DigiMobileResponseModel GetReservationDetails(int Id)
        {
            try
            {
                var spResponse =new DapperHelper().ExecuteSP<TempReservation>("Usp_GetReservationDetails", ConfigurationModel.ConnectionString,new { Id=Id });
               
                if (spResponse != null)
                {
                     return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = "Success",
                            ResponseData = spResponse.FirstOrDefault()
                        };
                   
                }
                else
                {
                    LogHelper.Instance.Debug("SP GetReservationDetails returned null", "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }
        public static DigiMobileResponseModel GetStatus(string StatusCode)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<NotificationStatus>("Usp_GetStatus", ConfigurationModel.ConnectionString, new { StatusCode = StatusCode });

                if (spResponse != null)
                {
                    return new DigiMobileResponseModel()
                    {
                        result = true,
                        ResultCode = "200",
                        ResponseMessage = "Success",
                        ResponseData = spResponse.FirstOrDefault()
                    };

                }
                else
                {
                    LogHelper.Instance.Debug("SP GetReservationDetails returned null", "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static DigiMobileResponseModel RejectReservation(int Id)
        {
            try
            {
                LogHelper.Instance.Debug("SP RejectReservation started", "DigiMobile", "Portal", "RejectReservation");
                var spResponse =
                                    new DapperHelper().ExecuteSP<SPResponseModel>("Usp_DeleteTempReservation", ConfigurationModel.ConnectionString,
                                        new { Id=Id })
                                   ;
                if (spResponse != null)
                {
                      LogHelper.Instance.Debug("SP RejectReservation "+spResponse, "DigiMobile", "Portal", "RejectReservation");
                        return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = "Success"
                        };
                 

                }
                else
                {
                    LogHelper.Instance.Debug("SP RejectReservation returned null", "DigiMobile", "Portal", "RejectReservation");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "RejectReservation", "DigiMobile", "RejectReservation");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }
        public static DigiMobileResponseModel CheckDocumentExistsDocument(TempReservationRequest documentRequest)
        {
            try
            {

                LogHelper.Instance.Debug($"document starting processing named" + documentRequest.TempReservationID, "ProcessDocument", "PortalAPI", "ProcessDocument");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                byte[] signatureBytes = Convert.FromBase64String(documentRequest.Base64Signature);

                var result = dapper.ExecuteSP<Models.Ereg.DocumentResponseModel>("Usp_GetDocumentfile", connectionstring, new
                {
                    DocumentDetailID=0,
                    DocumentType = documentRequest.DocumentType,
                   
                    ReservationNumber = documentRequest.ReservationNumber

                });




                LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.ReservationNumber, "ProcessDocument", "PortalAPI", "ProcessDocument");

                if (result != null)
                {
                    if (result.Count() > 0)
                    {
                        return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = $"success",
                            ResponseData = result.FirstOrDefault().DocumentHeaderID
                        };
                    }
                    else
                    {
                        return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = $"success",
                            ResponseData = 0
                        };
                    }
                }
                else
                {
                    return new DigiMobileResponseModel()
                    {
                        result = true,
                        ResultCode = "200",
                        ResponseMessage = $"success",
                        ResponseData =0
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }

        public static DigiMobileResponseModel ProcessDocument(TempReservationRequest documentRequest,string DocHeader=null)
        {
            try
            {

                LogHelper.Instance.Debug($"document starting processing named" + documentRequest.TempReservationID, "ProcessDocument", "PortalAPI", "ProcessDocument");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                byte[] signatureBytes = Convert.FromBase64String(documentRequest.Base64Signature);

                var result = dapper.ExecuteSP<Models.DocumentResponseModel>("Usp_UploadDocumentMobile", connectionstring, new ProcessDocumentRequestModel()
                {
                    UserName = "VPrinter",
                    DocumentFile = documentRequest.FolioTemplate,
                    DocumentFileName = documentRequest.ReservationNumber,
                    DocumentType = documentRequest.DocumentType,
                    SignatureFile=signatureBytes,
                    ReservationNumber=documentRequest.ReservationNumber,
                    ParentDocumentHeaderID= Convert.ToInt32(DocHeader),
                    Phonenumber=documentRequest.Phone,
                    EmailAddress=documentRequest.EmailAddress
                    ,
                    RoomNo = documentRequest.RoomNo,
                    ArrivalDate = documentRequest.ArrivalDate,
                    DepartureDate = documentRequest.DepartureDate,
                    GuestName = documentRequest.GuestName

                });



 
                LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.ReservationNumber, "ProcessDocument", "PortalAPI", "ProcessDocument");

                
                return new DigiMobileResponseModel()
                {
                    result = true,
                    ResultCode = "200",
                    ResponseMessage = $"success",
                    ResponseData = result.FirstOrDefault().DocumentDetailID
                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "ProcessDocument", "PortalAPI", "ProcessDocument");
               
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }
        public static DigiMobileResponseModel DeleteReservation(int Id)
        {
            try
            {
                var spResponse =
                                    new DapperHelper().ExecuteSP<SPResponseModel>("Usp_DeleteTempReservation", ConfigurationModel.ConnectionString,
                                        new { Id = Id })
                                   ;
                if (spResponse != null)
                {
                    if (spResponse.Count()>0)
                    {
                        return new DigiMobileResponseModel()
                        {
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                   else
                    { 
                        return new DigiMobileResponseModel()
                        {
                            result = false,
                            ResultCode = "201",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    
                }
                else
                {
                    LogHelper.Instance.Debug("SP DeleteReservation returned null", "DigiMobile", "Portal", "DeleteReservation");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "DeleteReservation", "DigiMobile", "DeleteReservation");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }
        public static DigiMobileResponseModel GetCountryList()
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<CountryMaster>("Usp_GetCountry", ConfigurationModel.ConnectionString);

                if (spResponse != null)
                {
                    return new DigiMobileResponseModel()
                    {
                        result = true,
                        ResultCode = "200",
                        ResponseMessage = "Success",
                        ResponseData = spResponse
                    };

                }
                else
                {
                    LogHelper.Instance.Debug("SP GetReservationDetails returned null", "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }

        public static DigiMobileResponseModel GetStatesList(int Id)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<StateMaster>("Usp_GetStates", ConfigurationModel.ConnectionString,new {Id=Id});

                if (spResponse != null)
                {
                    return new DigiMobileResponseModel()
                    {
                        result = true,
                        ResultCode = "200",
                        ResponseMessage = "Success",
                        ResponseData = spResponse
                    };

                }
                else
                {
                    LogHelper.Instance.Debug("SP GetReservationDetails returned null", "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                    return new DigiMobileResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "GetReservationDetails", "DigiMobile", "GetReservationDetails");
                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }
        public static DigiMobileResponseModel UpdateRegistration(TempReservationRequest documentRequest)
        {
            try
            {

                LogHelper.Instance.Debug($"document starting processing named" + documentRequest.TempReservationID, "UpdateRegistration", "PortalAPI", "UpdateRegistration");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                byte[] signatureBytes = Convert.FromBase64String(documentRequest.Base64Signature);

                var result = dapper.ExecuteSP<Models.Ereg.DocumentResponseModel>("Usp_UpdateReservationDetails", connectionstring, new
                {
                    CountryMasterID=documentRequest.CountryMasterID,
                    FlightNo=documentRequest.FlightNo,
                    City=documentRequest.City,
                    EmailAddress=documentRequest.EmailAddress,

                    PostalCode=documentRequest.PostalCode,

                    StateMasterID=documentRequest.StateMasterID,
                    Folio=documentRequest.FolioTemplate,
                    Phone =documentRequest.Phone,
                    AddressLine1=documentRequest.AddressLine1,
                    MembershipNo=documentRequest.MembershipNo,
                    TempId=documentRequest.TempReservationID,
                    SignatureFile= signatureBytes

                });




                LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.ReservationNumber, "ProcessDocument", "PortalAPI", "ProcessDocument");


                return new DigiMobileResponseModel()
                {
                    result = true,
                    ResultCode = "200",
                    ResponseMessage = $"success",
                    ResponseData = result.FirstOrDefault().DocumentDetailID
                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }

        public static DigiMobileResponseModel UpdateDocument(int docId, byte[] docfile)
        {
            try
            {

                LogHelper.Instance.Debug($"document starting processing named" + docId, "ProcessDocument", "PortalAPI", "ProcessDocument");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
               

                var result = dapper.ExecuteSP<Models.Ereg.DocumentResponseModel>("usp_UpdateDocumentFileDetails", connectionstring, new
                {
                    DocID = docId,
                    DocumentFile = docfile

                });




                LogHelper.Instance.Debug($"document inserted successfully to blob" + docId, "ProcessDocument", "PortalAPI", "ProcessDocument");


                return new DigiMobileResponseModel()
                {
                    result = true,
                    ResultCode = "200",
                    ResponseMessage = $"success",
                    ResponseData =docId
                };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Error" + ex, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new DigiMobileResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = $"Generic error : {ex.Message}"
                };
            }
        }
        }
}