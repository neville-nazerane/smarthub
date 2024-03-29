﻿using SmartHub.Models.Entities;
using SmartHub.Models.Models;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Consumer
{
    public class RaspberryClient
    {
        private readonly HttpClient _client;

        public RaspberryClient(HttpClient client)
        {
            _client = client;
        }

        #region settings

        public Task<IEnumerable<Setting>> GetSettingsAsync(CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<IEnumerable<Setting>>("settings", cancellationToken);

        public Task UpdateSettingAsync(Setting setting, CancellationToken cancellationToken = default)
            => _client.PutAsJsonAsync("settings", setting, cancellationToken);

        #endregion

        #region scenes

        public Task<IEnumerable<SceneState>> GetScenesAsync(CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<IEnumerable<SceneState>>("scenes", cancellationToken);

        public Task UpdateSceneAsync(SceneState.SceneNames sceneName,
                                     bool isEnabled,
                                     CancellationToken cancellationToken = default)
            => _client.PutAsync($"scene/{sceneName}/{isEnabled}", null, cancellationToken);

        #endregion

        #region devices
        public Task<IEnumerable<DeviceItem>> GetDevicesAsync(CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<IEnumerable<DeviceItem>>("devices", cancellationToken);

        public Task ExecuteDeviceAsync(string deviceId,
                                       DeviceExecuteModel model,
                                       CancellationToken cancellationToken = default)
            => _client.PostAsJsonAsync($"devices/{deviceId}/execute", model, cancellationToken);

        public Task<CapabilityData> GetCapabilityAsync(string id,
                                  float version,
                                  CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<CapabilityData>($"devices/capabilities/{id}/{version}", cancellationToken);

        public Task<Status> GetDeviceStatusAsync(string deviceId, string componentId, string capabilityId, CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<Status>($"devices/{deviceId}/components/{componentId}/capabilities/{capabilityId}/status");


        #endregion

        #region actions

        public Task<IEnumerable<ActionInfo>> GetActionsAsync(CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<IEnumerable<ActionInfo>>("actions", cancellationToken);

        public Task SetActionAsync(DeviceAction action, CancellationToken cancellationToken = default)
            => _client.PutAsJsonAsync("actions/device", action, cancellationToken);

        public Task SetActionAsync(SceneAction action, CancellationToken cancellationToken = default)
            => _client.PutAsJsonAsync("actions/scene", action, cancellationToken);

        public Task RemoveActionAsync(string id, CancellationToken cancellationToken = default)
            => _client.DeleteAsync($"actions/{id}", cancellationToken);

        public Task ExecuteActionAsync(string id, CancellationToken cancellationToken = default)
            => _client.PostAsync($"actions/execute/{id}", null, cancellationToken);

        #endregion

        #region bond

        public Task UpdateBondFanSpeedAsync(string fanId, int speed, CancellationToken cancellationToken = default)
            => _client.PutAsync($"bond/fanSpeed/{fanId}/{speed}", null, cancellationToken);

        public Task<int> GetBondFanSpeedAsync(string fanId, CancellationToken cancellationToken = default)
            => _client.GetFromJsonAsync<int>($"bond/fanSpeed/{fanId}", cancellationToken);

        public Task SwitchBondLightAsync(string lightId, bool on, CancellationToken cancellationToken = default)
            => _client.PutAsync($"bond/light/{lightId}/{on}", null, cancellationToken);
        
        public async Task<bool> GetBondLightAsync(string lightId, CancellationToken cancellationToken = default)
        {
            var res = await _client.GetAsync($"bond/light/{lightId}", cancellationToken);
            return bool.Parse(await res.Content.ReadAsStringAsync());
        }

        #endregion


        public Task SetHueLight(string id,
                            bool switchOn,
                            CancellationToken cancellationToken = default)
            => _client.PutAsync($"/hue/light/{id}/{switchOn}", null, cancellationToken);

        public async Task<bool> GetHueLight(string id,
                            CancellationToken _ = default)
            => bool.Parse(await _client.GetStringAsync($"/hue/light/{id}"));

    }
}
