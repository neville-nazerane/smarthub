using Microsoft.EntityFrameworkCore;
using SmartHub.Constants;
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
    public class ScenesService
    {
        private readonly AppDbContext _dbContext;
        private readonly HueClient _hueClient;
        private readonly BondClient _bondClient;
        private readonly SmartThingsClient _smartThingsClient;

        public ScenesService(AppDbContext dbContext, HueClient hueClient, BondClient bondClient, SmartThingsClient smartThingsClient)
        {
            _dbContext = dbContext;
            _hueClient = hueClient;
            _bondClient = bondClient;
            _smartThingsClient = smartThingsClient;
        }

        public async Task<IEnumerable<SceneState>> GetAsync(CancellationToken cancellationToken = default)
        {
            var scenes = await _dbContext.SceneStates.ToListAsync(cancellationToken);
            var missing = Enum.GetNames<SceneState.SceneNames>()
                              .Except(scenes.Select(s => s.SceneName))
                              .Select(s => new SceneState
                              {
                                  SceneName = s
                              })
                              .ToList();

            var notNeeded = scenes.Where(s => !Enum.GetNames<SceneState.SceneNames>().Contains(s.SceneName));
            _dbContext.RemoveRange(notNeeded);
            await _dbContext.SceneStates.AddRangeAsync(missing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            scenes.AddRange(missing);

            return scenes;
        }

        public Task<bool> GetSceneEnabledStateAsync(SceneState.SceneNames sceneName,
                                                          CancellationToken cancellationToken = default)
            => _dbContext.SceneStates
                         .AsNoTracking()
                         .Where(s => s.SceneName == sceneName.ToString())
                         .Select(s => s.IsEnabled)
                         .SingleOrDefaultAsync(cancellationToken);

        public async Task UpdateAsync(SceneState.SceneNames sceneName,
                                      bool isEnabled,
                                      CancellationToken cancellationToken = default)
        {
            await UpdateInternalAsync(sceneName, isEnabled, false, cancellationToken);
        }

        private async Task UpdateInternalAsync(SceneState.SceneNames sceneName, bool isEnabled, bool suppressTouchingScenes, CancellationToken cancellationToken)
        {
            switch (sceneName)
            {
                case SceneState.SceneNames.Goodnight:
                    await ExecuteGoodNightAsync(isEnabled, suppressTouchingScenes, cancellationToken);
                    break;
                case SceneState.SceneNames.Snooze:
                    await ExecuteSnoozeAsync(isEnabled, suppressTouchingScenes, cancellationToken);
                    break;
            }

            var scene = await _dbContext.SceneStates.SingleAsync(s => s.SceneName == sceneName.ToString(), cancellationToken: cancellationToken);
            scene.IsEnabled = isEnabled;
            _dbContext.SceneStates.Update(scene);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ExecuteSnoozeAsync(bool isEnabled, bool suppressTouchingScenes, CancellationToken cancellationToken = default)
        {
            if (!suppressTouchingScenes)
            {
                bool goodNightEnabled = await GetSceneEnabledStateAsync(SceneState.SceneNames.Goodnight, cancellationToken);
                if (goodNightEnabled)
                    await UpdateInternalAsync(SceneState.SceneNames.Goodnight, false, true, cancellationToken); 
            }

            if (isEnabled)
            {
                await Task.WhenAll(
                    _hueClient.SwitchLightAsync(DeviceConstants.hueComputerLightId, false, cancellationToken),
                    _bondClient.ToggleLightAsync(DeviceConstants.bondBedFanId, false, cancellationToken),
                    _smartThingsClient.ExecuteSceneAsync(SceneConstants.CloseFrontRoom, cancellationToken)
                );
            }
            else
            {
                await Task.WhenAll(
                    _hueClient.SwitchLightAsync(DeviceConstants.hueComputerLightId, true, cancellationToken),
                    _bondClient.ToggleLightAsync(DeviceConstants.bondBedFanId, true, cancellationToken),
                    _smartThingsClient.ExecuteSceneAsync(SceneConstants.OpenFrontRoom, cancellationToken)
                );
            }
        }

        private async Task ExecuteGoodNightAsync(bool isEnabled, bool suppressTouchingScenes, CancellationToken cancellationToken = default)
        {
            if (!suppressTouchingScenes)
            {
                bool snoozeEnabled = await GetSceneEnabledStateAsync(SceneState.SceneNames.Snooze, cancellationToken);
                if (snoozeEnabled)
                    await UpdateInternalAsync(SceneState.SceneNames.Snooze, false, true, cancellationToken); 
            }

            if (isEnabled)
            {
                await Task.WhenAll(
                    _hueClient.SwitchLightAsync(DeviceConstants.hueComputerLightId, false, cancellationToken),
                    _bondClient.ToggleLightAsync(DeviceConstants.bondBedFanId, false, cancellationToken),
                    _smartThingsClient.ExecuteSceneAsync(SceneConstants.CloseFrontRoom, cancellationToken)
                );
            }
            else
            {
                await Task.WhenAll(
                    _hueClient.SwitchLightAsync(DeviceConstants.hueComputerLightId, true, cancellationToken),
                    _bondClient.ToggleLightAsync(DeviceConstants.bondBedFanId, true, cancellationToken),
                    _smartThingsClient.ExecuteSceneAsync(SceneConstants.OpenFrontRoom, cancellationToken)
                );
            }
        }

    }
}
