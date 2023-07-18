using DigiDoc.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DigiDoc.DataAccess;
using DigiDoc.WebAPI.Helper;
using DigiDoc.Helper;
using Azure.Storage.Blobs;
using System.Configuration;

namespace DigiDoc.WebAPI.Controllers
{
    public class DocumentController : ApiController
    {
        [HttpPost]
      
        public async Task<ServiceResponseModel> ProcessDocument(DocumentRequestModel documentRequest)
        {
            try
            {
                
                LogHelper.Instance.Debug($"document starting processing named"+documentRequest.DocumentName, "ProcessDocument", "PortalAPI", "ProcessDocument");
                string connectionstring = ConfigurationModel.ConnectionString;
                DapperHelper dapper = new DapperHelper();
                byte[] documentBytes = Convert.FromBase64String(documentRequest.DocumentBase64);

             var result=   dapper.ExecuteSP<DocumentResponseModel>("Usp_UploadDocuments", connectionstring, new
                {
                    UserName = documentRequest.Username,
                    DocumentFile = documentBytes,
                    DocumentFileName = documentRequest.DocumentName,
                    DocumentType = "Other Documents",
                });
               


                string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
               
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);

            
                await new BlobStorage().UploadFileBlobAsync(documentBytes, "document" + result.FirstOrDefault().DocumentDetailID + ".pdf", blobServiceClient);

                 LogHelper.Instance.Debug($"document inserted successfully to blob" + documentRequest.DocumentName, "ProcessDocument", "PortalAPI", "ProcessDocument");

                return new ServiceResponseModel()
                {
                    Result = true,
                    ResponseCode = "200",
                    ResponseMessage = $"success",
                    
                };
            }
            catch(Exception ex)
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
    }
}
