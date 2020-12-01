using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class EventService
    {
        private readonly AutomationService _automationService;
        private readonly EventLogService _eventLogService;

        public EventService(AutomationService automationService, EventLogService eventLogService)
        {
            _automationService = automationService;
            _eventLogService = eventLogService;
        }

        public async Task RecieveAsync(string @event, CancellationToken cancellationToken = default)
        {
            await _eventLogService.LogAsync(@event, cancellationToken);
            await _automationService.ExecuteForAsync(@event, cancellationToken);
        }

    }
}
