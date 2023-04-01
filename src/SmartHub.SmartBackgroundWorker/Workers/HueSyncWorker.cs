using SmartHub.SmartBackgroundWorker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Workers
{
    public class HueSyncWorker : BackgroundService
    {
        private readonly HueSyncProcess _hueSyncProcess;

        public HueSyncWorker(HueSyncProcess hueSyncProcess)
        {
            _hueSyncProcess = hueSyncProcess;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _hueSyncProcess.RunAsync(stoppingToken);
                }
                catch (Exception ex) 
                {
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
