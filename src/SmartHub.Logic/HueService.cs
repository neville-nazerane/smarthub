using SmartHub.Models.Hue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class HueService
    {
        private readonly HueClient _client;

        public HueService(HueClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> WatchIncomingAsync()
        {
            var res = await _client.StreamEventAsync();
            Console.WriteLine(res);
            return res;
        }

        public async Task HandleEventAsync(HttpResponseMessage response)
        {

            string rawData = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JsonSerializer.Serialize(JsonDocument.Parse(rawData), new JsonSerializerOptions { WriteIndented = true }));

            var data = JsonSerializer.Deserialize<IEnumerable<HueEvent>>(rawData);

            response.Dispose();

            //Console.WriteLine("\n\n\n\n\n\n\n\nHANDLING");
            //Console.WriteLine(eve);
            //await Task.Delay(3000);
            //Console.WriteLine("\n\nDone handling");
            //Console.WriteLine(eve);
        }

    }
}
