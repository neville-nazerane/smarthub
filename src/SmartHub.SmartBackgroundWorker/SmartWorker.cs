using SmartHub.Logic;
using System.Text.Json;

namespace SmartHub.SmartBackgroundWorker
{
    public class SmartWorker : BackgroundService
    {
        private int frontReq = 75;

        private readonly ILogger<SmartWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SmartWorker(ILogger<SmartWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await using var scope = _serviceProvider.CreateAsyncScope();
                var provider = scope.ServiceProvider;

                await ControlFrontRoomAsync(provider.GetService<SmartThingsClient>(), stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private async Task ControlFrontRoomAsync(SmartThingsClient client,
                                                 CancellationToken cancellationToken = default)
        {

            var frontTempData = await client.GetCapabilityStatusAsync("5583ad3b-6d21-4826-90bf-be8ba43a48f5",
                                                                    "main",
                                                                    "temperatureMeasurement",
                                                                    cancellationToken);

            var fanData = await client.GetCapabilityStatusAsync("21b89395-01f3-49b4-b284-6530c0022b61",
                                                                "main",
                                                                "fanSpeed",
                                                                cancellationToken);

            float currentTemp = frontTempData["temperature"]["value"].GetSingle();
            int fanSpeed = fanData["fanSpeed"]["value"].GetInt32();

            if (currentTemp < frontReq)
            {
                if (fanSpeed > 1)
                {
                    --fanSpeed;
                    await IncreaseFanAsync();
                }
                else if (fanSpeed == 1)
                    await SwitchFanAsync("off");
            }
            else if (currentTemp > frontReq + 1)
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
                await client.ExecuteDeviceAsync("21b89395-01f3-49b4-b284-6530c0022b61", new Models.SmartThings.DeviceExecuteModel
                {
                    Component = "main",
                    Capability = "fanSpeed",
                    Command = "setFanSpeed",
                    Arguments = new object[] { fanSpeed }
                }, cancellationToken);
            }

            async Task SwitchFanAsync(string value)
            {
                await client.ExecuteDeviceAsync("21b89395-01f3-49b4-b284-6530c0022b61", new Models.SmartThings.DeviceExecuteModel
                {
                    Component = "main",
                    Capability = "switch",
                    Command = value,
                }, cancellationToken);
            }

        }
    }
}