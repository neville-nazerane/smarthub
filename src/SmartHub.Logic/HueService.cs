using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<string> WatchIncomingAsync()
        {
            var res = await _client.StreamEventAsync();
            Console.WriteLine(res);
            return res;
        }

        public async Task HandleEventAsync(string eve)
        {
            Console.WriteLine("\n\n\n\n\n\n\n\nHANDLING");
            Console.WriteLine(eve);
            await Task.Delay(3000);
            Console.WriteLine("\n\nDone handling");
            Console.WriteLine(eve);
        }

    }
}
