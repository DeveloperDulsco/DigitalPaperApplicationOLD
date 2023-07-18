using Azure;
using Azure.Core;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DigiDoc.WebAPI.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiDoc.Helper
{
    public class BlobStorage
    {
        string blobContainer = ConfigurationManager.AppSettings["ContainerName"];

        /// <summary>
        /// get azure blob connection
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <param name="accountName"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tenantID"></param>
        public static void GetBlobServiceClientAzureAD(ref BlobServiceClient blobServiceClient)
        {

            string ConnectionString = ConfigurationManager.AppSettings["CloudConnectionString"];
            blobServiceClient = new BlobServiceClient(ConnectionString);
        }
        /// <summary>
        /// create sample container
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <returns></returns>
        private static async Task<BlobContainerClient> CreateSampleContainerAsync(BlobServiceClient blobServiceClient)
        {
            // Name the sample container based on new GUID to ensure uniqueness.
            // The container name must be lowercase.
            string containerName = ConfigurationManager.AppSettings["ContainerName"];

            try
            {
                // Create the container
                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        /// Delete container
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static async Task DeleteSampleContainerAsync(BlobServiceClient blobServiceClient, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

            try
            {
                // Delete the specified container and handle the exception.
                await container.DeleteAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Restore a container
        /// </summary>
        /// <param name="client"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static async Task RestoreContainer(BlobServiceClient client, string containerName)
        {
            foreach (BlobContainerItem item in client.GetBlobContainers
               (BlobContainerTraits.None, BlobContainerStates.Deleted))
            {
                if (item.Name == containerName && (item.IsDeleted == true))
                {
                    try
                    {
                        await client.UndeleteBlobContainerAsync(containerName, item.VersionId);
                    }
                    catch (RequestFailedException e)
                    {
                        Console.WriteLine("HTTP error code {0}: {1}",
                        e.Status, e.ErrorCode);
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// upload by file stream
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public async Task UploadStream
    (BlobServiceClient containerClient, string localFilePath, string fileName)
        {
            LogHelper.Instance.Debug($"document UploadStream" + fileName, "ProcessDocument", "PortalAPI", "ProcessDocument");

            var client = GetContainerClient(containerClient);
            // string fileName = Path.GetFileName(localFilePath);
            BlobClient blobClient = client.GetBlobClient(fileName);


            FileStream fileStream = File.OpenRead(localFilePath);
            LogHelper.Instance.Debug($"document inserted successfully" + fileStream, "ProcessDocument", "PortalAPI", "ProcessDocument");
            await blobClient.UploadAsync(fileStream, true);
            fileStream.Close();
        }

        /// <summary>
        /// Upload by File path
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public async Task UploadFile
    (string fileName, BlobServiceClient client, string localFilePath)
        {
            var containerClient = GetContainerClient(client);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(localFilePath, true);
        }
        /// <summary>
        /// Download by file path from blob
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static async Task DownloadBlob(BlobClient blobClient, string localFilePath)
        {
            await blobClient.DownloadToAsync(localFilePath);
        }
        /// <summary>
        /// Download from a stream
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static async Task DownloadfromStream(BlobClient blobClient, string localFilePath)
        {
            using (var stream = await blobClient.OpenReadAsync())
            {
                FileStream fileStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(fileStream);
            }
        }
        /// <summary>
        /// Download to a stream
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static async Task DownloadToStream(BlobClient blobClient, string localFilePath)
        {
            FileStream fileStream = File.OpenWrite(localFilePath);
            await blobClient.DownloadToAsync(fileStream);
            fileStream.Close();
        }

        public async Task<BlobContentInfo> UploadFileBlobAsync(byte[] content, string fileName, BlobServiceClient client)
        {
            try
            {
                LogHelper.Instance.Debug($"document uploaded to cloud starting:file name=" + fileName, "UploadFileBlobAsync", "PortalAPI", "ProcessDocument");
                BlobContentInfo contentinfo = null;
                using (Stream stream = new MemoryStream(content))
                {
                    var containerClient = GetContainerClient(client);
                    var blobClient = containerClient.GetBlobClient(fileName);
                    contentinfo = await blobClient.UploadAsync(stream);
                    LogHelper.Instance.Debug($"document uploaded to cloud successfully" + contentinfo.BlobSequenceNumber, "UploadFileBlobAsync", "PortalAPI", "ProcessDocument");
                }
                return contentinfo;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Debug($"Exception" + ex, "UploadFileBlobAsync", "PortalAPI", "ProcessDocument");
                ex.ToString();
            }
            return null;
        }

        private BlobContainerClient GetContainerClient(BlobServiceClient client)
        {
            var containerClient = client.GetBlobContainerClient(blobContainer.ToString());
            containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient;
        }
        public async Task<byte[]> GetFileBlobAsync(string fileName, BlobServiceClient client)
        {
            try
            {
                var containerClient = GetContainerClient(client);
                var blobClient = containerClient.GetBlobClient(fileName);

                if (blobClient.Exists())
                {
                    var response = await blobClient.DownloadAsync();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        response.Value.Content.CopyTo(stream);

                        return stream.ToArray();
                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return null;
        }
        public async Task DeleteFile
   (BlobServiceClient client, string fileName)
        {

            var containerClient = GetContainerClient(client);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
   
