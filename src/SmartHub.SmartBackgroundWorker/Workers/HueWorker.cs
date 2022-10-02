using SmartHub.Logic;
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
        private readonly HueClient _hueClient;
        private readonly IServiceProvider _serviceProvider;

        public HueWorker(HueClient hueClient, IServiceProvider serviceProvider)
        {
            _hueClient = hueClient;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
                // sync scene names
                await scope.ServiceProvider.GetService<ScenesService>().GetAsync(stoppingToken);

            await CallbackHandler.BeginRunningAsync(_hueClient.StreamEventAsync, HandleEventAsync);
        }

        Task HandleEventAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                return scope.ServiceProvider.GetService<HueProcessor>().HandleEventAsync(response, cancellationToken);
            }
            catch 
            {
                return Task.CompletedTask;
            }
        }

    }
}
