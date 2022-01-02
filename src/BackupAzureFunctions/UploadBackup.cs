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

namespace SmartHub.BackupAzureFunctions
{
    public static class UploadBackup
    {

        [FunctionName("UploadBackup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploadBackup/{name}")] HttpRequest req,
            [Blob("backups/{name}.zip", FileAccess.Write)] Stream stream,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await req.Body.CopyToAsync(stream);

            return new OkObjectResult("Uploaded!");
        }
    }
}