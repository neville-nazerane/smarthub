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
    public class ScenesRepo
    {
        private readonly AppDbContext _dbContext;

        public ScenesRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task UpdateAsync(SceneState.SceneNames sceneName,
                                      bool isEnabled,
                                      CancellationToken cancellationToken = default)
        {
            var scene = await _dbContext.SceneStates.SingleAsync(s => s.SceneName == sceneName.ToString(), cancellationToken: cancellationToken);
            scene.IsEnabled = isEnabled;
            _dbContext.SceneStates.Update(scene);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
