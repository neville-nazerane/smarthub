using SmartHub.Logic;
using SmartHub.SmartBackgroundWorker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.SmartBackgroundWorker
{
    public class HueWorker : BackgroundService
    {
        private readonly HueService _hueService;

        public HueWorker(HueService hueService)
        {
            _hueService = hueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handler = new ParallelHandler();

            await handler.BeginRunningAsync(_hueService.WatchIncomingAsync,
                                            _hueService.HandleEventAsync);

            //var tasks = Enumerable.Range(1, 3)
            //                      .Select(i => handler.RunAsync(() => _hueService.IncomingAsync()))
            //                      .ToArray();

            //int justInt = tasks.Count();

            //return Task.WhenAll(tasks);
        }


    }
}
