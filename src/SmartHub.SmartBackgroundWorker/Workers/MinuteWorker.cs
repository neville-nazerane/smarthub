using SmartHub.SmartBackgroundWorker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Workers
{
    public class MinuteWorker : BackgroundService
    {
        private readonly MinuiteProcessor _minuiteProcessor;

        public MinuteWorker(MinuiteProcessor minuiteProcessor)
        {
            _minuiteProcessor = minuiteProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.WhenAll(
                    _minuiteProcessor.RunForNowAsync(),
                    Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)
                );
            }
        }
    }
}
