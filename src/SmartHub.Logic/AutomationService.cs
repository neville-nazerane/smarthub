using Microsoft.Extensions.Logging;
using SmartHub.Logic.Automations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartHub.Logic.Automations.AutomationsCollection;

namespace SmartHub.Logic
{
    public class AutomationService
    {

        private static readonly AutomationsCollection _automations;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutomationService> _logger;

        static AutomationService()
        {
            _automations = new AutomationsCollection()
                                .SetAutomation<TurnOnBedroomAutomation>(EventTypes.BedroomMotion);
        }

        public AutomationService(IServiceProvider serviceProvider, ILogger<AutomationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ExecuteForAsync(string @event, CancellationToken cancellationToken = default)
        {
            var autos = _automations.Get(@event);

            if (autos is null)
            {
                _logger.LogWarning("No actions set for events recieved", @event);
                return;
            }

            _logger.LogInformation("Found {event} automations for event {count}", @event, autos.Count());
            if (autos is not null)
                foreach (var auto in autos)
                {
                    var service = (IAutomation)_serviceProvider.GetService(auto);
                    await service.ExecuteAsync(cancellationToken);
                }
            else _logger.LogWarning("No automations found for event", @event);
        }

    }
}
