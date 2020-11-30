using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartHub.WebApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Endpoints
{
    public static class SceneEndpoints
    {

        public static IEndpointConventionBuilder MapScenes(this IEndpointRouteBuilder endpoints, string path)
            => new MultiEndpointConventionBuilder { 
                
                endpoints.MapPost(path, async context
                    => await context.Response
                                    .WriteAsJsonAsync(
                                            await context.GetSmartThingsClient()
                                                         .GetScenesAsync()))

            };

    }
}
