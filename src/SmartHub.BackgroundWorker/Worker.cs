using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHub.Logic;

namespace SmartHub.BackgroundWorker
{
    public class Worker : BackgroundService
    {

        private readonly TimeSpan logClearInterval = TimeSpan.FromHours(1);

        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetService<EventLogService>();

                await service.ClearLogsAsync(logClearInterval, stoppingToken);

                await Task.Delay(logClearInterval, stoppingToken);
            }
        }
    }
}
