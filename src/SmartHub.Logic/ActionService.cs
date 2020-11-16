using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartHub.Logic.Data;
using SmartHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Logic
{
    public class ActionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActionService> _logger;

        public ActionService(AppDbContext context, ILogger<ActionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SetAsync(DeviceAction action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await RemoveAsync(action.Id);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, $"Failed to remove '{action.Id}'");
                return;
            }


            await _context.DeviceActions.AddAsync(action);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Failed to add new action");
                return;
            }
            await transaction.CommitAsync();
        }

        public async Task RemoveAsync(string id)
        {
            var record = await _context.DeviceActions.SingleOrDefaultAsync(d => d.Id == id);
            if (record is not null)
            {
                _context.Remove(record);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"No record found to remove for '{id}'");
            }
        }

    }
}
