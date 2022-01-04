using Microsoft.EntityFrameworkCore;
using SmartHub.Logic;
using SmartHub.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker
{
    public class SmartyPants
    {
        private const string frontTempKey = "front temp";
        private const string bedTempKey = "bedroom temp";

        private const string frontMonitorId = "5583ad3b-6d21-4826-90bf-be8ba43a48f5";
        private const string frontFanId = "21b89395-01f3-49b4-b284-6530c0022b61";

        private const string bedMonitorId = "d177e94a-37b6-4f34-90fb-e872139505ee";
        private const string bedFanId = "bf836338-a339-4e9a-90e6-e147de8ead17";



        private readonly SmartThingsClient _smartThingsClient;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<SmartyPants> _logger;

        public SmartyPants(SmartThingsClient smartThingsClient,
                           AppDbContext dbContext,
                           ILogger<SmartyPants> logger)
        {
            _smartThingsClient = smartThingsClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ExecuteBedroomFanAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int setting = await _dbContext.GetSettingAsIntAsync(bedTempKey, cancellationToken);
                await SpinFanAsync(bedFanId, bedMonitorId, setting, cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Could not find setting '{bedTempKey}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute bedroom fan stuff");
            }
        }

        public async Task ExecuteFrontFanAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int setting = await _dbContext.GetSettingAsIntAsync(frontTempKey, cancellationToken);
                await SpinFanAsync(frontFanId, frontMonitorId, setting, cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Could not find setting '{frontTempKey}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute front fan stuff");
            }
        }

        private async Task SpinFanAsync(string fanId,
                                        string monitorId,
                                        int targetTemp,
                                        CancellationToken cancellationToken = default)
        {

            var frontTempData = await _smartThingsClient.GetCapabilityStatusAsync(monitorId,
                                                                    "main",
                                                                    "temperatureMeasurement",
                                                                    cancellationToken);

            var fanData = await _smartThingsClient.GetCapabilityStatusAsync(fanId,
                                                                "main",
                                                                "fanSpeed",
                                                                cancellationToken);

            float currentTemp = frontTempData["temperature"]["value"].GetSingle();
            int fanSpeed = fanData["fanSpeed"]["value"].GetInt32();

            if (currentTemp < targetTemp)
            {
                if (fanSpeed > 1)
                {
                    --fanSpeed;
                    await IncreaseFanAsync();
                }
                else if (fanSpeed == 1)
                    await SwitchFanAsync("off");
            }
            else if (currentTemp > targetTemp + 1)
            {
                if (fanSpeed == 0)
                    await SwitchFanAsync("on");
                else if (fanSpeed < 3)
                {
                    ++fanSpeed;
                    await IncreaseFanAsync();
                }
            }

            async Task IncreaseFanAsync()
            {
                await _smartThingsClient.ExecuteDeviceAsync(fanId, new Models.SmartThings.DeviceExecuteModel
                {
                    Component = "main",
                    Capability = "fanSpeed",
                    Command = "setFanSpeed",
                    Arguments = new object[] { fanSpeed }
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
