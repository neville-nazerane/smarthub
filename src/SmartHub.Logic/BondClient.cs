using SmartHub.Logic.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class BondClient
    {
        private readonly HttpClient _httpClient;

        public BondClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SetFanSpeedAsync(string fanId,
                                     int speed,
                                     CancellationToken cancellationToken = default)
        {
            var str = JsonSerializer.Serialize(new BondArgument
            {
                Argument = speed
            });
            var content = new StringContent(str, Encoding.UTF8, "application/json");
            var res = await _httpClient.PutAsync($"/v2/devices/{fanId}/actions/SetSpeed", content, cancellationToken);
            res.EnsureSuccessStatusCode();
        }

        public async Task ToggleFanAsync(string fanId, bool on, CancellationToken cancellationToken = default)
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var path = on ? "TurnOn" : "TurnOff";

            var res = await _httpClient.PutAsync($"/v2/devices/{fanId}/actions/{path}", content, cancellationToken);
            res.EnsureSuccessStatusCode();
        }

        public async Task ToggleLightAsync(string id, bool on, CancellationToken cancellationToken = default)
        {
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var path = on ? "TurnLightOn" : "TurnLightOff";

            var res = await _httpClient.PutAsync($"/v2/devices/{id}/actions/{path}", content, cancellationToken);
            res.EnsureSuccessStatusCode();
        }

        public Task<BondState> GetStateAsync(string fanId, CancellationToken cancellationToken = default)
            => _httpClient.GetFromJsonAsync<BondState>($"v2/devices/{fanId}/state", cancellationToken);

        public async Task IncreaseFanAsync(string fanId, CancellationToken cancellationToken = default)
        {
            var state = await GetStateAsync(fanId, cancellationToken);
            if (state.Speed < 3)
            {
                if (state.Power == 0)
                {
                    await SetFanSpeedAsync(fanId, 1, cancellationToken);
                    await ToggleFanAsync(fanId, true, cancellationToken);
                }
                else await SetFanSpeedAsync(fanId, state.Speed + 1, cancellationToken); 
            }
        }

        public async Task DecreaseFanAsync(string fanId, CancellationToken cancellationToken = default)
        {
            var state = await GetStateAsync(fanId, cancellationToken);
            if (state.Speed > 0)
            {
                if (state.Speed == 1)
                    await ToggleFanAsync(fanId, false, cancellationToken);
                else
                    await SetFanSpeedAsync(fanId, state.Speed - 1, cancellationToken);
            }
        }

    }

}
