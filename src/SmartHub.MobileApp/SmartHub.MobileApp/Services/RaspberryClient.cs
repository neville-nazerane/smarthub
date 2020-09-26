using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.MobileApp.Services
{
    public class RaspberryClient
    {
        private readonly HttpClient _client;

        public RaspberryClient(HttpClient client)
        {
            _client = client;
        }

        public Task<IEnumerable<DeviceItem>> GetDevicesAsync() 
            =>  _client.GetFromJsonAsync<IEnumerable<DeviceItem>>("devices");

    }
}
