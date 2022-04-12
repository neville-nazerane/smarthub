using SmartHub.Logic.InternalModels;
using SmartHub.Models.Hue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class HueClient
    {
        private readonly HttpClient _httpClient;

        public HueClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task SwitchLightAsync(string id, bool switchOn, CancellationToken cancellationToken = default)
        {
            var model = new LightSwitch
            {
                On = new LightOnOff_OnModel
                {
                    On = switchOn
                }
            };
            return _httpClient.PutAsJsonAsync($"/clip/v2/resource/light/{id}", model, cancellationToken);
        }

        public Task<HueData<LightSwitch>> GetLightInfoAsync(string id, CancellationToken cancellationToken = default)
            => _httpClient.GetFromJsonAsync<HueData<LightSwitch>>($"/clip/v2/resource/light/{id}", cancellationToken);

        public async Task<HttpResponseMessage> StreamEventAsync(CancellationToken cancellationToken = default)
        {
            var streamTime = Stopwatch.StartNew();
            HttpResponseMessage res = await _httpClient.GetAsync("eventstream/clip/v2", cancellationToken);
            return res;
        }


    }
}
