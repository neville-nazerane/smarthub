using SmartHub.Models;
using SmartHub.Models.SmartThings;
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
        public readonly HttpClient _httpClient;

        public SmartThingsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region scenes

        public async Task<IEnumerable<SceneItem>> GetScenesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<SceneData>("scenes");
            return result.Items;
        }

        public Task ExecuteSceneAsync(string id) => _httpClient.PostAsync($"scenes/{id}/execute", null);

        #endregion


        #region devices

        public async Task<IEnumerable<DeviceItem>> GetDevicesAsync()
        {
            var data = await _httpClient.GetFromJsonAsync<DeviceData>("devices");
            return data.Items;
        }

        public Task ExecuteDeviceAsync(string deviceId, params DeviceExecuteModel[] models) 
            => _httpClient.PostAsJsonAsync($"/devices/{deviceId}/commands", new { commands = models });
 
        #endregion
    }
}
 