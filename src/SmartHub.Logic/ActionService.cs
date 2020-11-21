using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartHub.Logic.Data;
using SmartHub.Models.Entities;
using SmartHub.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class ActionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActionService> _logger;

        enum ActionId 
        {
            turnOnBedroom
        }

        public ActionService(AppDbContext context, ILogger<ActionService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async ValueTask<IEnumerable<ActionInfo>> GetActionsInfoAsync(CancellationToken cancellationToken = default)
        {
            var ids = Enum.GetValues<ActionId>().Select(i => i.ToString());
            var actions = await _context.DeviceActions
                                        .Where(d => ids.Contains(d.Id))
                                        .Select(d => d.Id)
                                        .ToArrayAsync(cancellationToken);

            var result = ids.Select(id => new ActionInfo { Id = id, IsSet = actions.Contains(id) });

            return result;
        }
        public async Task SetAsync(DeviceAction action, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await RemoveAsync(action.Id, cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, $"Failed to remove '{action.Id}'");
                return;
            }

            await _context.DeviceActions.AddAsync(action, cancellationToken);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(e, "Failed to add new action");
                return;
            }
            await transaction.CommitAsync(cancellationToken);
        }
        public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            var record = await _context.DeviceActions.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (record is not null)
            {
                _context.Remove(record);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning($"No record found to remove for '{id}'");
            }
        }

    }
}
