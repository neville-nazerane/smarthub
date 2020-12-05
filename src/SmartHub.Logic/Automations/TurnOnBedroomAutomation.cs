using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHub.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartHub.Logic.Automations.AutomationsCollection;

namespace SmartHub.Logic.Automations
{
    public class TurnOnBedroomAutomation : IAutomation
    {
        private const string startTimeStr = "8:00 AM";
        private const string endTimeStr = "2:00 AM";
        private const string timeFormat = "h:mm tt";

        private static readonly TimeSpan aWhile = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan startTime = DateTime.ParseExact(startTimeStr, timeFormat, null).TimeOfDay;
        private static readonly TimeSpan endTime = DateTime.ParseExact(endTimeStr, timeFormat, null).TimeOfDay;

        private readonly ILogger<TurnOnBedroomAutomation> _logger;
        private readonly AppDbContext _context;
        private readonly ActionService _actionService;

        public TurnOnBedroomAutomation(ILogger<TurnOnBedroomAutomation> logger,
                                       AppDbContext context,
                                       ActionService actionService)
        {
            _logger = logger;
            _context = context;
            _actionService = actionService;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Triggered turn on automation");
            var current = DateTime.Now.TimeOfDay;
            if (current > startTime && current < endTime)
            {
                string motion = EventTypes.BedroomMotion.ToString();
                string noMotion = EventTypes.BedroomNoMotion.ToString();

                var verifyTime = DateTime.UtcNow.Subtract(aWhile);
                int recentCount = await _context.EventLogs
                                                    .CountAsync(e => e.EventId == motion && e.TimeStamp > verifyTime, 
                                                              cancellationToken: cancellationToken);

                var motions = new string[] { motion, noMotion };

                if (recentCount < 2)
                    await _actionService.ExecuteActionAsync(ActionService.ActionId.turnOnBedroom, cancellationToken);
            }
            _logger.LogInformation("Finished turn on automation");
        }

    }
}
