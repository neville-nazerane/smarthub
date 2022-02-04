using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Storage.Blobs;
using System.Collections.Generic;
using Azure.Storage.Blobs.Models;

namespace SmartHub.BackupAzureFunctions
{
    public static class UploadBackup
    {
        private const string dateFormat = "yy-MM-dd-HH-mm";

        [FunctionName("UploadBackup")]
        public static async Task<IActionResult> BackupEndpoint(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadBackup/{name}")] HttpRequest req,
            [Blob("backups/{name}.zip", FileAccess.Write)] Stream stream,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await req.Body.CopyToAsync(stream);

            return new OkObjectResult("Uploaded!");
        }

        [FunctionName("BackupCleanupPolicy")]
        public static async Task AutoCleanup(
            [TimerTrigger("0 0 0 * * *", RunOnStartup = true)] TimerInfo timer
            ,[Blob("backups", FileAccess.Read)]BlobContainerClient blobReader
            ,[Blob("backups", FileAccess.Write)] BlobContainerClient backupClient
            ,[Blob("archives", FileAccess.Write)] BlobContainerClient archiveClient
            )
        {

            var pages = blobReader.GetBlobsAsync();

            var blobsToMove = new List<BlobItem>();
            var current = DateTime.Now;
            await foreach (var blob in pages)
            {
                var date = DateTime.ParseExact(blob.Name, dateFormat, null);
                if ((date - current).Days > 7)
                {
                    blobsToMove.Add(blob);
                }
            }

            foreach (var blob in blobsToMove)
            {
                await using var stream = (await blobReader.GetBlobClient(blob.Name).DownloadStreamingAsync()).Value.Content;
                string blobName = $"Weekly-{blob.Name}";
                var newBlob = await archiveClient.UploadBlobAsync(blobName, stream);
                await archiveClient.GetBlobClient(blobName).SetAccessTierAsync(AccessTier.Archive);
                await backupClient.DeleteBlobAsync(blob.Name);
            }


        }

    }
}