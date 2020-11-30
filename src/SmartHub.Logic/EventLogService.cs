using Microsoft.EntityFrameworkCore;
using SmartHub.Logic.Data;
using SmartHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class EventLogService
    {
        private readonly AppDbContext _context;

        public EventLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string eventId, CancellationToken cancellationToken = default)
        {
            await _context.EventLogs.AddAsync(new EventLog { 
                EventId = eventId, 
                TimeStamp = DateTime.UtcNow
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventLog>> GetAsync(string eventId, CancellationToken cancellationToken = default)
            => await _context.EventLogs
                                .Where(l => l.EventId == eventId)
                                .ToArrayAsync(cancellationToken);

        public async Task ClearLogsAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            var time = DateTime.UtcNow.Subtract(timeSpan);
            await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"DELETE FROM {nameof(_context.EventLogs)} WHERE {nameof(EventLog.TimeStamp)} < {time}",
                            cancellationToken);
        }

    }
}
