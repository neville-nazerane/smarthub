using SmartHub.Models.Hue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SmartHub.Constants;

namespace SmartHub.Logic
{
    public class HueService
    {

        private const string computerHaloId = "404b22ea-8b2f-43ed-93ff-3641f5c478d5";
        private const string buttonId = "419bf6d0-02d5-4932-bc03-b761c9ecbb71";

        private readonly HueClient _client;
        private readonly SmartThingsClient _smartThingsClient;

        public HueService(HueClient client, SmartThingsClient smartThingsClient)
        {
            _client = client;
            _smartThingsClient = smartThingsClient;
        }

        public Task<HttpResponseMessage> WatchIncomingAsync(CancellationToken cancellationToken = default)
            => _client.StreamEventAsync(cancellationToken);

        public async Task HandleEventAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var data = await response.Content.ReadFromJsonAsync<IEnumerable<HueEvent>>(cancellationToken: cancellationToken);
            response.Dispose();
            IEnumerable<HueEventData> events = data.Where(d => d.Type == "update")
                                                         .SelectMany(d => d.Data)
                                                         .ToList();
            Console.WriteLine(events.FirstOrDefault()?.Button?.LastEvent);
            await ProcessEventAsync(events, cancellationToken);
        }

        private async Task ProcessEventAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken = default)
        {
            var buttonEvent = events.SingleOrDefault(s => s.Id == buttonId);
            Console.WriteLine(buttonEvent?.Button?.LastEvent);
            if (buttonEvent is not null)
            {
                switch (buttonEvent.Button.LastEvent)
                {
                    case "short_release":
                        await SwitchComputerHaloAsync(cancellationToken);
                        break;
                    case "long_release":

                        var stateResult = await _smartThingsClient.GetCapabilityStatusAsync(DeviceConstants.pcSwapId, "main", "switch", cancellationToken);
                        string state = stateResult["switch"]["value"].GetString();
                        string newState = state == "on" ? "off" : "on";

                        await _smartThingsClient.ExecuteDeviceAsync(DeviceConstants.pcSwapId, new Models.SmartThings.DeviceExecuteModel
                        {
                            Component = "main",
                            Capability = "switch",
                            Command = newState
                        }, cancellationToken);

                        break;
                }
            }
        }

        private async Task SwitchComputerHaloAsync(CancellationToken cancellationToken = default)
        {
            var button = await _client.GetLightInfoAsync(computerHaloId, cancellationToken);
            var newState = !button.Data.First().On.On;

            await _client.SwitchLightAsync(computerHaloId, newState, cancellationToken);
        }

    }
}
