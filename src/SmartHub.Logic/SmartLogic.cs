using SmartHub.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class SmartLogic
    {

        private static DateTime lastFanCommand;

        private readonly SmartThingsClient _smartThingsClient;

        public SmartLogic(SmartThingsClient smartThingsClient)
        {
            _smartThingsClient = smartThingsClient;
        }

        public async Task ExecuteRawAutomation(string @event, CancellationToken cancellationToken = default)
        {
            switch (@event)
            {
                case "increaseFan":
                    await UpdateFanOnAllRoomsAsync(true, cancellationToken);
                    break;
                case "decreaseFan":
                    await UpdateFanOnAllRoomsAsync(false, cancellationToken);
                    break;
            }
        }

        private static HttpClient testClient = new HttpClient
        {
            BaseAddress = new Uri("https://webhook.site")
        };

        private async Task UpdateFanOnAllRoomsAsync(bool isIncreased, CancellationToken cancellationToken = default)
        {
            await testClient.GetAsync("c29c0e8d-49e0-48ac-9dc6-e149c9179d0a?status=" + isIncreased, cancellationToken);
            if ((DateTime.UtcNow - lastFanCommand).Seconds < 2) return;
            lastFanCommand = DateTime.UtcNow;
            await UpdateFanByRoomAsync(DeviceConstants.frontSwitchId,
                                       DeviceConstants.frontFanId,
                                       isIncreased,
                                       cancellationToken);

            await UpdateFanByRoomAsync(DeviceConstants.bedGhostId,
                                       DeviceConstants.bedFanId,
                                       isIncreased,
                                       cancellationToken);

        }

        private async Task UpdateFanByRoomAsync(string roomId, string fanId, bool isIncreased, CancellationToken cancellationToken = default)
        {
            var roomStatusData = await _smartThingsClient.GetCapabilityStatusAsync(roomId,
                                                                                   "main",
                                                                                   "switch",
                                                                                   cancellationToken);
            if (roomStatusData["switch"]["value"].GetString() == "on")
                await UpdateFanSpeedAsync(fanId, isIncreased, cancellationToken);
        }

        private async Task UpdateFanSpeedAsync(string fanId,
                                               bool isIncreased,
                                               CancellationToken cancellationToken = default)
        {
            var fanData = await _smartThingsClient.GetCapabilityStatusAsync(fanId,
                                                    "main",
                                                    "fanSpeed",
                                                    cancellationToken);

            int fanSpeed = fanData["fanSpeed"]["value"].GetInt32();

            int newSpeed = fanSpeed + (isIncreased ? 1 : -1);
            await SetFanSpeedAsync(fanId, newSpeed, cancellationToken);
        }

        public async Task SetFanSpeedAsync(string fanId, int speed, CancellationToken cancellationToken = default)
        {

            var fanData = await _smartThingsClient.GetCapabilityStatusAsync(fanId,
                                                    "main",
                                                    "fanSpeed",
                                                    cancellationToken);
            int fanSpeed = fanData["fanSpeed"]["value"].GetInt32();

            if (speed == 0)
            {
                if (fanSpeed > 0)
                    await SwitchFanAsync("off");
            }
            else
            {

                await IncreaseFanAsync();
            }

            async Task IncreaseFanAsync()
            {
                await _smartThingsClient.ExecuteDeviceAsync(fanId, new Models.SmartThings.DeviceExecuteModel
                {
                    Component = "main",
                    Capability = "fanSpeed",
                    Command = "setFanSpeed",
                    Arguments = new object[] { speed }
                }, cancellationToken);
            }

            async Task SwitchFanAsync(string value)
            {
                await _smartThingsClient.ExecuteDeviceAsync(fanId, new Models.SmartThings.DeviceExecuteModel
                {
                    Component = "main",
                    Capability = "switch",
                    Command = value,
                }, cancellationToken);
            }

        }

    }
}
