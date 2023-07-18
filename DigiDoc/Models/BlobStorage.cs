using Azure;
using Azure.Core;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        public static async Task UploadStream
    (BlobContainerClient containerClient, string localFilePath)
        {
            string fileName = Path.GetFileName(localFilePath);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            FileStream fileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(fileStream, true);
            fileStream.Close();
        }

        /// <summary>
        /// Upload by File path
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public static async Task UploadFile
    (BlobContainerClient containerClient, string localFilePath)
        {
            string fileName = Path.GetFileName(localFilePath);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(localFilePath, true);
        }
        /// <summary>
        /// Download by file path from blob
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public  async Task<int> DownloadBlob(BlobServiceClient client, string localFilePath, string fileName)
        {
            var containerClient = GetContainerClient(client);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DownloadToAsync(localFilePath);
            return 1;
        }
        /// <summary>
        /// Download from a stream
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public  async Task DownloadfromStream(BlobServiceClient client, string localFilePath, string fileName)
        {
            var containerClient = GetContainerClient(client);
            var blobClient = containerClient.GetBlobClient(fileName);
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
        public  async Task<byte[]> DownloadToStream(BlobServiceClient client, string localFilePath,string fileName)
        {
            var containerClient = GetContainerClient(client);
            var blobClient = containerClient.GetBlobClient(fileName);
            FileStream fileStream = File.OpenWrite(localFilePath);
            await blobClient.DownloadToAsync(fileStream);
            byte[] value = null;
            using (MemoryStream ms = new MemoryStream())
            {
              
                LogHelper.Instance.Debug($"filstream value" + fileStream, "ProcessDocument", "Portal", "ProcessDocument");            
                fileStream.Position = 0;
                fileStream.CopyTo(ms);
                LogHelper.Instance.Debug($"filstream value" + ms, "ProcessDocument", "Portal", "ProcessDocument");

                value = ms.ToArray();
            }
            
            fileStream.Close();
           
            return value;
        }

        public async Task<BlobContentInfo> UploadFileBlobAsync(byte[] content, string fileName, BlobServiceClient client)
        {
            try
            {
                BlobContentInfo s = null;
            using (Stream stream = new MemoryStream(content))
            {
                var containerClient = GetContainerClient(client);
                var blobClient = containerClient.GetBlobClient(fileName);
              s=  await blobClient.UploadAsync(stream);
            }
            return s;
            }
            catch (Exception ex)
            {
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
                LogHelper.Instance.Debug($"GetFileBlobAsync", "ProcessDocument", "Portal", "ProcessDocument");
                var containerClient = GetContainerClient(client);
                LogHelper.Instance.Debug($"GetContainerClient" + containerClient.Uri, "ProcessDocument", "Portal", "ProcessDocument");

                var blobClient = containerClient.GetBlobClient(fileName);

                LogHelper.Instance.Debug($"GetBlobClient" + blobClient.BlobContainerName, "ProcessDocument", "Portal", "ProcessDocument");
              
              //  FileStream fileStream = null;
                LogHelper.Instance.Debug($"fileStream" + blobClient.Name, "ProcessDocument", "Portal", "ProcessDocument");

                //FileStream fileStreams = File.OpenWrite(@"C:\temp\testqqs.pdf");
                //LogHelper.Instance.Debug($"fileStream creates" + fileStreams, "ProcessDocument", "PortalAPI", "ProcessDocument");

                //await blobClient.DownloadToAsync(fileStreams);
                //fileStream.Close();
                //LogHelper.Instance.Debug($"GetBlobClient" + fileStreams, "ProcessDocument", "PortalAPI", "ProcessDocument");
                // var streams = await blobClient.OpenReadAsync();
                //LogHelper.Instance.Debug($"GetBlobClient" + streams, "ProcessDocument", "PortalAPI", "ProcessDocument");

                //using (var stream = await blobClient.OpenReadAsync().ConfigureAwait(false))
                //{
                //    LogHelper.Instance.Debug($"OpenReadAsync" + blobClient.BlobContainerName, "ProcessDocument", "PortalAPI", "ProcessDocument");

                //    fileStream = File.OpenWrite(@"C:\temp\testqq.pdf");
                //    await stream.CopyToAsync(fileStream);

                //}
                //Byte[] bytes = File.ReadAllBytes(@"C:\temp\testqq.pdf");

                //return bytes;

              
                    LogHelper.Instance.Debug($"await blob", "ProcessDocument", "Portal", "ProcessDocument");
                  //  var stream = await blobClient.OpenReadAsync().ConfigureAwait(false);
               
                    
                using (var ms = new MemoryStream())
                {
                    LogHelper.Instance.Debug($"await blob", "ProcessDocument", "Portal", "ProcessDocument");

                       await blobClient.DownloadToAsync(ms);
                    
                    ms.Position = 0;
                    LogHelper.Instance.Debug($"document return" + ms.ToArray(), "ProcessDocument", "Portal", "ProcessDocument");

                    return ms.ToArray();
                }
                // response = await blobClient.DownloadAsync();
               // LogHelper.Instance.Debug($"document return" + ms.ToArray(), "ProcessDocument", "PortalAPI", "ProcessDocument");
                //LogHelper.Instance.Debug($"document inserted successfully" + response.Value.Content, "ProcessDocument", "PortalAPI", "ProcessDocument");
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    response.Value.Content.CopyTo(stream);

                //    return stream.ToArray();
                //}
                //return stream.WriteByte();


            }
            catch (Exception ex)

            {
                LogHelper.Instance.Debug($"ex"+ex, "ProcessDocument", "Portal", "ProcessDocument");
                ex.ToString();
            }
            return null;
        }
        public byte[] FileToByteArray(string fileName)
        {
            LogHelper.Instance.Debug($"document read started" + fileName, "ProcessDocument", "Portal", "ProcessDocument");
            byte[] fileData = null;
            if (File.Exists(fileName))
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    LogHelper.Instance.Debug($"document filestream started" + fs.Length, "ProcessDocument", "Portal", "ProcessDocument");

                    using (BinaryReader binaryReader = new BinaryReader(fs))
                    {
                        fileData = binaryReader.ReadBytes((int)fs.Length);
                    }
                }
                File.Delete(fileName);
            }
            return fileData;
        }

        /// <summary>
        /// Upload by File path
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public  async Task DeleteFile
    (BlobServiceClient client, string fileName)
        {

            var containerClient = GetContainerClient(client);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}