using SmartHub.Models;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class SmartThingsClient
    {
        public readonly HttpClient _httpClient;

        public SmartThingsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region scenes

        public async Task<IEnumerable<SceneItem>> GetScenesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.GetFromJsonAsync<SceneData>("scenes", cancellationToken);
            return result.Items;
        }

        public Task ExecuteSceneAsync(string id, CancellationToken cancellationToken = default) 
            => _httpClient.PostAsync($"scenes/{id}/execute", null, cancellationToken);

        #endregion

        #region devices

        public async Task<IEnumerable<DeviceItem>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            var data = await _httpClient.GetFromJsonAsync<DeviceData>("devices", cancellationToken);
            return data.Items;
        }

        public Task ExecuteDeviceAsync(string deviceId, DeviceExecuteModel model, CancellationToken cancellationToken = default)
            => ExecuteDeviceAsync(deviceId, new DeviceExecuteModel[] { model }, cancellationToken);

        public async Task ExecuteDeviceAsync(string deviceId, DeviceExecuteModel[] models, CancellationToken cancellationToken = default)
        {
            var res = await _httpClient.PostAsJsonAsync($"/devices/{deviceId}/commands", new { commands = models }, cancellationToken);
            res.EnsureSuccessStatusCode();
        }

        public Task<object> GetCapabilityStatusAsync(string deviceId, string componentId, string capabilityId, CancellationToken cancellationToken = default)
            => _httpClient.GetFromJsonAsync<object>($"/devices/{deviceId}/components/{componentId}/capabilities/{capabilityId}/status", cancellationToken);

        #endregion

        #region capabilities

        public Task<CapabilityData> GetCapabilityAsync(string id, float version, CancellationToken cancellationToken = default)
            => _httpClient.GetFromJsonAsync<CapabilityData>($"/capabilities/{id}/{version}", cancellationToken);
        #endregion

    }
}
