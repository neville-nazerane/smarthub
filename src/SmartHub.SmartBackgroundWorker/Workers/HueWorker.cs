using SmartHub.SmartBackgroundWorker.Services;
using SmartHub.SmartBackgroundWorker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker.Workers
{
    public class HueWorker : BackgroundService
    {
        private readonly HueProcessor _hueService;

        public HueWorker(HueProcessor hueService)
        {
            _hueService = hueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await CallbackHandler.BeginRunningAsync(_hueService.WatchIncomingAsync,
                                                     _hueService.HandleEventAsync);
        }


    }
}
