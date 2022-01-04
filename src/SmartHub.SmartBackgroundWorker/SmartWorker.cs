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
                var service = scope.ServiceProvider.GetService<SmartyPants>();

                await Task.WhenAll(
                    service.ExecuteFrontFanAsync(stoppingToken),
                    service.ExecuteBedroomFanAsync(stoppingToken)
                );

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        
    }
}