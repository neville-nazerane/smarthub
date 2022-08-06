using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartHub.Logic;
using SmartHub.Models.Entities;
using SmartHub.WebApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Endpoints
{
    public static class SceneEndpoints
    {

        public static IEndpointConventionBuilder MapScenes(this IEndpointRouteBuilder endpoints)
            => new MultiEndpointConventionBuilder { 
                
                endpoints.MapGet("/scenes", GetScenesAsync),
                endpoints.MapPut("/scene/{sceneName}/{isEnabled}", UpdateSceneAsync)
            
            };

        static Task<IEnumerable<SceneState>> GetScenesAsync(ScenesRepo repo, 
                                                            CancellationToken cancellationToken = default)
            => repo.GetAsync(cancellationToken);

        static Task UpdateSceneAsync(ScenesRepo repo,
                                     SceneState.SceneNames sceneName,
                                     bool isEnabled,
                                     CancellationToken cancellationToken = default)
            => repo.UpdateAsync(sceneName, isEnabled, cancellationToken);

    }
}
