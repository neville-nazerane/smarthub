using SmartHub.Models.HueSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class HueSyncClient
    {
        private readonly HttpClient _httpClient;

        public HueSyncClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ExecutionResult> GetExecutionAsync(CancellationToken cancellationToken = default)
            => _httpClient.GetFromJsonAsync<ExecutionResult>("api/v1/execution", cancellationToken);

        //public Task UpdateExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken = default)
        //    => _httpClient.PutAsJsonAsync("api/v1/execution", request, cancellationToken);


        public Task UpdateExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken = default)
        {
            string s = JsonSerializer.Serialize(request);
            var content = new StringContent(s, Encoding.UTF8, "application/json");
            return _httpClient.PutAsync("api/v1/execution", content, cancellationToken);
        }
    }
}
