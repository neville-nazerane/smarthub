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
        private static readonly TimeSpan aWhile = TimeSpan.FromMinutes(5);

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

            string motion = EventTypes.BedroomMotion.ToString();
            string noMotion = EventTypes.BedroomNoMotion.ToString();

            var verifyTime = DateTime.UtcNow.Subtract(aWhile);
            int recentCount = await _context.EventLogs
                                                .CountAsync(e => e.EventId == motion && e.TimeStamp > verifyTime, 
                                                          cancellationToken: cancellationToken);

            var motions = new string[] { motion, noMotion };

            //var lastMotionEvent = await _context.EventLogs
            //                                  .Where(e => motions.Contains(e.EventId))
            //                                  .OrderByDescending(e => e.TimeStamp)
            //                                  .Select(e => e.EventId)
            //                                  .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (recentCount < 2)
            {
                await _actionService.ExecuteActionAsync(ActionService.ActionId.turnOnBedroom, cancellationToken);
            }
            _logger.LogInformation("Finished turn on automation");
        }

    }
}
