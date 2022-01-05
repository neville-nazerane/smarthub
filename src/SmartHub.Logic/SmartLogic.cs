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
                if (fanSpeed == 0)
                {
                    await SwitchFanAsync("on");
                }

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
