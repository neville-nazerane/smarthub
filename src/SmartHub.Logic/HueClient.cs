using SmartHub.Models.Hue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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

    }
}
