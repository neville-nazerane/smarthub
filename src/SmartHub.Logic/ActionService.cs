using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SmartHub.Logic.Data;
using SmartHub.Models.Entities;
using SmartHub.Models.Models;
using SmartHub.Models.SmartThings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly SmartThingsClient _smartThingsClient;

        public enum ActionId 
        {
            turnOnBedroom, alexaKnock, offFront
        }

        public ActionService(AppDbContext context, ILogger<ActionService> logger, SmartThingsClient smartThingsClient)
        {
            _context = context;
            _logger = logger;
            _smartThingsClient = smartThingsClient;
        }

        public async ValueTask<IEnumerable<ActionInfo>> GetActionsInfoAsync(CancellationToken cancellationToken = default)
        {
            var ids = Enum.GetValues<ActionId>().Select(i => i.ToString());
            var deviceActions = await _context.DeviceActions
                                                .Where(d => ids.Contains(d.Id))
                                                .Select(d => d.Id)
                                                .ToArrayAsync(cancellationToken);
            var sceneActions = await _context.SceneActions
                                                .Where(d => ids.Contains(d.Id))
                                                .Select(d => d.Id)
                                                .ToArrayAsync(cancellationToken);

            var actions = deviceActions.Union(sceneActions);

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

        public async Task SetAsync(SceneAction action, CancellationToken cancellationToken = default)
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

            await _context.SceneActions.AddAsync(action, cancellationToken);
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
            var device = await _context.DeviceActions.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (device is not null)
                _context.DeviceActions.Remove(device);

            else
            {
                var scene = await _context.SceneActions.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
                if (scene is not null)
                    _context.SceneActions.Remove(scene);
                else
                {
                    _logger.LogWarning($"No record found to remove for '{id}'");
                    return;
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> ExecuteActionAsync(ActionId actionId, CancellationToken cancellationToken = default)
            => ExecuteActionAsync(actionId.ToString(), cancellationToken);

        public async Task<bool> ExecuteActionAsync(string id, CancellationToken cancellationToken = default)
        {
            var deviceAction = await _context.DeviceActions.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (deviceAction is not null)
            {
                await _smartThingsClient.ExecuteDeviceAsync(deviceAction.DeviceId, new DeviceExecuteModel
                { 
                    Capability = deviceAction.Capability,
                    Command = deviceAction.Command,
                    Component = deviceAction.Component
                }, cancellationToken);
                return true;
            }

            var sceneAction = await _context.SceneActions.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (sceneAction is not null)
            {
                await _smartThingsClient.ExecuteSceneAsync(sceneAction.SceneId, cancellationToken);
                return true;
            }
            return false;
        }

    }
}
