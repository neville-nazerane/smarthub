﻿using SmartHub.Models.Hue;
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
using SmartHub.Models.Entities;

namespace SmartHub.SmartBackgroundWorker.Services
{
    public class HueProcessor
    {
        private const string closetKillerTimer = "closetKiller";
        private readonly HueClient _client;
        private readonly SmartThingsClient _smartThingsClient;
        private readonly MinuiteProcessor _minuiteProcessor;
        private readonly ScenesService _scenesService;
        private readonly BondClient _bondClient;

        private static readonly IEnumerable<string> extraPcLightIds = new string[]
        {
            DeviceConstants.computerRightBarId,
            DeviceConstants.computerLeftBarId,
            DeviceConstants.computerLeftHaloId,
            DeviceConstants.computerRightHaloId,
        };

        public HueProcessor(HueClient client,
                            SmartThingsClient smartThingsClient,
                            MinuiteProcessor minuiteProcessor,
                            ScenesService scenesService,
                            BondClient bondClient)
        {
            _client = client;
            _smartThingsClient = smartThingsClient;
            _minuiteProcessor = minuiteProcessor;
            _scenesService = scenesService;
            _bondClient = bondClient;
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
            try
            {
                await ProcessEventAsync(events, cancellationToken);
            }
            catch
            {
            }
        }

        private async Task ProcessEventAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken = default)
        {
            //await VerifyPcButtonAsync(events, cancellationToken);

            //await VerifyIncreaseButtonAsync(events, cancellationToken);
            //await VerifyDecreaseButtonAsync(events);
            //await VerifyClosetMotionAsync(events, cancellationToken);
            //await VerifyClosetLightAsync(events);

            //await VerifyComputerLightAsync(events, cancellationToken);
            //await VerifyBedroomPowerButtonAsync(events, cancellationToken);
        }

        Task VerifyBedroomPowerButtonAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken = default)
        {
            var button = events.SingleOrDefault(s => s.Id == DeviceConstants.hueBedroomPowerId);
            if (button is not null)
            {
                Console.WriteLine("BLOCK " + button.Button.LastEvent);
                switch (button.Button.LastEvent)
                {
                    case "short_release":
                        return SwitchSceneAsync(SceneState.SceneNames.Snooze, cancellationToken);
                    case "long_release":
                        return SwitchSceneAsync(SceneState.SceneNames.Goodnight, cancellationToken);
                    default:

                        break;
                }
            }
            return Task.CompletedTask;
        }

        async Task SwitchSceneAsync(SceneState.SceneNames scene, CancellationToken cancellationToken = default)
        {
            bool isEnabled = await _scenesService.GetSceneEnabledStateAsync(scene, cancellationToken);
            await _scenesService.UpdateAsync(scene, !isEnabled, cancellationToken);
        }

        async ValueTask VerifyComputerLightAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken)
        {
            var light = events.LastOrDefault(e => e.Id == DeviceConstants.hueComputerLightPlugId);
            if (light is not null)
            {
                foreach (var id in extraPcLightIds)
                    await _client.SwitchLightAsync(id, light.On, cancellationToken);
                //return Task.WhenAll(
                //    extraPcLightIds.Select(id => _client.SwitchLightAsync(id, light.On, cancellationToken)).ToList()    
                //);
            }
        }

        async Task VerifyClosetMotionAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken)
        {
            var closetMotion = events.LastOrDefault(e => e.Id == DeviceConstants.hueCloestMotionId);
            if (closetMotion is not null)
            {
                if (closetMotion.Motion.Motion)
                {
                    await _client.SwitchLightAsync(DeviceConstants.closetLightId, true, cancellationToken);
                    await _minuiteProcessor.RemoveAsync(closetKillerTimer);
                }
                else
                {
                    await ClosetLightAutoOffAsync();
                }
            }
        }

        Task VerifyClosetLightAsync(IEnumerable<HueEventData> events)
        {
            var closetLight = events.LastOrDefault(e => e.Id == DeviceConstants.closetLightId);
            if (closetLight is not null)
            {
                if (closetLight.On)
                    return ClosetLightAutoOffAsync();
                else
                    return _minuiteProcessor.RemoveAsync(closetKillerTimer);
            }
            return Task.CompletedTask;
        }

        Task ClosetLightAutoOffAsync()
        {
            return _minuiteProcessor.AddAsync(closetKillerTimer, DateTime.Now.AddMinutes(5),
                                                                 () => _client.SwitchLightAsync(DeviceConstants.closetLightId, false, CancellationToken.None));
        }

        private Task VerifyPcButtonAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken)
        {
            var buttonEvent = events.SingleOrDefault(s => s.Id == DeviceConstants.buttonId);
            if (buttonEvent is not null)
            {
                switch (buttonEvent.Button.LastEvent)
                {
                    case "short_release":
                        return SwitchComputerHaloAsync(cancellationToken);
                    case "repeat":
                        return SwitchPcControlsAsync(cancellationToken);
                }
            }
            return Task.CompletedTask;
        }

        private async Task SwitchPcControlsAsync(CancellationToken cancellationToken)
        {
            var stateResult = await _smartThingsClient.GetCapabilityStatusAsync(DeviceConstants.pcSwapId, "main", "switch", cancellationToken);
            string state = stateResult["switch"]["value"].GetString();
            string newState = state == "on" ? "off" : "on";

            await _smartThingsClient.ExecuteDeviceAsync(DeviceConstants.pcSwapId, new Models.SmartThings.DeviceExecuteModel
            {
                Component = "main",
                Capability = "switch",
                Command = newState
            }, cancellationToken);
        }

        private async Task VerifyDecreaseButtonAsync(IEnumerable<HueEventData> events)
        {
            var decreaseButton = events.SingleOrDefault(s => s.Id == DeviceConstants.hueBedroomDecreaseId);
            if (decreaseButton is not null)
                await _bondClient.DecreaseFanAsync(DeviceConstants.bondBedFanId);
        }

        private async Task VerifyIncreaseButtonAsync(IEnumerable<HueEventData> events, CancellationToken cancellationToken = default)
        {
            var increaseButton = events.SingleOrDefault(s => s.Id == DeviceConstants.hueBedroomIncreaseId);
            if (increaseButton is not null)
                await _bondClient.IncreaseFanAsync(DeviceConstants.bondBedFanId, cancellationToken);
        }

        private async Task SwitchComputerHaloAsync(CancellationToken cancellationToken = default)
        {
            var dictionary = new ConcurrentDictionary<string, bool>();

            var tasks = new string[]
            {
                DeviceConstants.hueComputerLightPlugId
            }
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

            foreach (var task in tasks)
                await task;
            //await Task.WhenAll(tasks);

            var exeTasks = dictionary.Select(kv => _client.SwitchLightAsync(kv.Key, kv.Value, cancellationToken));

            foreach (var exeTask in exeTasks) 
                await exeTask;
            //await Task.WhenAll(exeTasks);

            //var button = await _client.GetLightInfoAsync(DeviceConstants.computerHaloId, cancellationToken);
            //var newState = !button.Data.First().On.On;

            //await _client.SwitchLightAsync(DeviceConstants.computerHaloId, newState, cancellationToken);
        }

    }
}
