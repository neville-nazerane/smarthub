using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class AzBackupClient
    {
        private readonly HttpClient _httpClient;

        public AzBackupClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task UploadAsync(Stream fileStream, CancellationToken cancellationToken = default)
        {
            //var request = new HttpRequestMessage(HttpMethod.Post, 
            //                                    $"uploadBackup/{DateTime.Now:yy-MM-dd-HH-mm}");

            //request.Content = new StreamContent(fileStream);

            return _httpClient.PostAsync($"uploadBackup/{DateTime.Now:yy-MM-dd-HH-mm}",
                                         new StreamContent(fileStream),
                                         cancellationToken);
        }

    }
}
