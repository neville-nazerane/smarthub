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
        private const string timeFormat = "h:mm tt";

        private static readonly TimeSpan aWhile = TimeSpan.FromMinutes(5);
        private static readonly double startTime = GetMilliseconds("8:00 AM");
        private static readonly double endTime = GetMilliseconds("2:00 AM");

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

        static double GetMilliseconds(string timeStr)
            => DateTime.ParseExact(timeStr, timeFormat, null).TimeOfDay.TotalMilliseconds;

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Triggered turn on automation");
            if (IsWithinTimeRange(DateTime.Now))
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

        private static bool IsWithinTimeRange(DateTime time)
        {
            var count = time.TimeOfDay.TotalMilliseconds;
            return startTime < endTime && count >= startTime && count <= endTime
                // endtime is next day
                   || (startTime > endTime && (count <= endTime || count >= startTime));
        }
            //=> (startTime.Hour < time.Hour || (startTime.Hour == time.Hour && startTime.Minute <= time.Minute))
            //  && (endTime.Hour > time.Hour || (endTime.Hour == time.Hour && endTime.Minute >= time.Minute));

    }
}
