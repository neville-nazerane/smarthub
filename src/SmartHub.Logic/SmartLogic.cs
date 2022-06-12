using SmartHub.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class SmartLogic
    {
        private readonly SmartThingsClient _smartThingsClient;

        public SmartLogic(SmartThingsClient smartThingsClient)
        {
            _smartThingsClient = smartThingsClient;
        }

        public async Task ExecuteRawAutomation(string @event, CancellationToken cancellationToken = default)
        {
            switch (@event)
            {
                case "increaseFrontFan":
                    await UpdateFanSpeedAsync(DeviceConstants.frontFanId, true, cancellationToken);
                    break;
                case "decreaseFrontFan":
                    await UpdateFanSpeedAsync(DeviceConstants.frontFanId, false, cancellationToken);
                    break;
                case "increaseBedroomFan":
                    await UpdateFanSpeedAsync(DeviceConstants.bedFanId, true, cancellationToken);
                    break;
                case "decreaseBedroomFan":
                    await UpdateFanSpeedAsync(DeviceConstants.bedFanId, false, cancellationToken);
                    break;
            }
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
