using SmartHub.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Services
{
    public class SmartThingsClient
    {
        private readonly HttpClient _httpClient;

        public SmartThingsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SceneItem>> GetScenesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<SceneData>("scenes");
            return result.Items;
        }

    }
}
