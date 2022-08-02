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
using System.Collections.Concurrent;
using System.Timers;
using SmartHub.Logic;
using SmartHub.SmartBackgroundWorker.Utils;

namespace SmartHub.SmartBackgroundWorker.Services
{
    public class HueProcessor
    {
        private const string closetKillerTimer = "closetKiller";
        private readonly HueClient _client;
        private readonly SmartThingsClient _smartThingsClient;
        private readonly MinuiteProcessor _minuiteProcessor;

        public HueProcessor(HueClient client, SmartThingsClient smartThingsClient, MinuiteProcessor minuiteProcessor)
        {
            _client = client;
            _smartThingsClient = smartThingsClient;
            _minuiteProcessor = minuiteProcessor;
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
            await ProcessEventAsync(events, cancellationToken);
        }

        private async Task ProcessEventAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken = default)
        {
            var buttonEvent = events.SingleOrDefault(s => s.Id == DeviceConstants.buttonId);
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

            var closetMotion = events.LastOrDefault(e => e.Id == DeviceConstants.hueCloestMotionId);
            if (closetMotion is not null)
            {
                //if (closetMotion.Motion.Motion)
                //{
                //    await _client.SwitchLightAsync(DeviceConstants.closetLightId, true, cancellationToken);
                //    await _minuiteProcessor.RemoveAsync(closetKillerTimer);
                //}
                //else
                //{
                    
                //}
            }

            var closetLight = events.LastOrDefault(e => e.Id == DeviceConstants.closetLightId);
            if (closetLight is not null)
            {
                if (closetLight.On)
                {
                    await _minuiteProcessor.AddAsync(closetKillerTimer, DateTime.Now.AddMinutes(5),
                                                     () => _client.SwitchLightAsync(DeviceConstants.closetLightId, false, CancellationToken.None));
                }
                else
                {
                    await _minuiteProcessor.RemoveAsync(closetKillerTimer);
                }
            }
        }

        private async Task SwitchComputerHaloAsync(CancellationToken cancellationToken = default)
        {
            var dictionary = new ConcurrentDictionary<string, bool>();

            var tasks = new string[]{ DeviceConstants.computerHaloId,
                                  DeviceConstants.computerRightBarId,
                                  DeviceConstants.computerLeftBarId }
                                 .Select(d => Task.Run(async () =>
                                 {
                                     try
                                     {
                                         var button = await _client.GetLightInfoAsync(d, cancellationToken);
                                         var newState = !button.Data.First().On.On;
                                         dictionary.TryAdd(d, newState);
                                     }
                                     catch
                                     {

                                     }
                                 }))
                                 .ToList();

            await Task.WhenAll(tasks);

            var exeTasks = dictionary.Select(kv => _client.SwitchLightAsync(kv.Key, kv.Value, cancellationToken));

            await Task.WhenAll(exeTasks);

            //var button = await _client.GetLightInfoAsync(DeviceConstants.computerHaloId, cancellationToken);
            //var newState = !button.Data.First().On.On;

            //await _client.SwitchLightAsync(DeviceConstants.computerHaloId, newState, cancellationToken);
        }

    }
}
