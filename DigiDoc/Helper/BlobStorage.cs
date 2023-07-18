using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiDoc.Helper
{
    public class BlobStorage
    {
        /// <summary>
        /// GetBlobService
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <param name="accountName"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tenantID"></param>
        public static void GetBlobServiceConnection(ref BlobServiceClient)
        {
            var connectionString = ConfigurationManager.AppSettings["CloudConnectionString"].ToString();

            //TokenCredential credential = new ClientSecretCredential(
            //    tenantID, clientID, clientSecret, new TokenCredentialOptions());

            //string blobUri = "https://" + accountName + ".blob.core.windows.net";

            blobServiceClient = new BlobServiceClient(connectionString)
        }
        /// <summary>
        /// get azure blob connection
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <param name="accountName"></param>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="tenantID"></param>
        public static void GetBlobServiceClientAzureAD(ref BlobServiceClient blobServiceClient,
    string accountName, string clientID, string clientSecret, string tenantID)
        {

            TokenCredential credential = new ClientSecretCredential(
                tenantID, clientID, clientSecret, new TokenCredentialOptions());

            string blobUri = "https://" + accountName + ".blob.core.windows.net";

            blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
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
            string containerName = "container-" + Guid.NewGuid();

            try
            {
                // Create the container
                BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                {
                    Console.WriteLine("Created container {0}", container.Name);
                    return container;
                }
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


    }
}