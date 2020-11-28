using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartHub.Logic;
using SmartHub.WebApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartHub.Models.Entities;

namespace SmartHub.WebApp.Endpoints
{
    public static class ActionsEndpoints
    {

        public static IEndpointConventionBuilder MapActions(this IEndpointRouteBuilder endpoints, string path)
            => new MultiEndpointConventionBuilder 
            {

                endpoints.MapGet(path, async context
                    => await context.Response
                                    .WriteAsJsonAsync(
                                                await context.GetAction()
                                                             .GetActionsInfoAsync(context.RequestAborted), 
                                                context.RequestAborted)),

                endpoints.MapPut($"{path}/device", async context 
                    => await context.GetAction()
                                    .SetAsync(await context.Request
                                                           .ReadFromJsonAsync<DeviceAction>(context.RequestAborted),
                                              context.RequestAborted)),

                endpoints.MapDelete($"{path}/{{id}}", context
                    => context.GetAction()
                              .RemoveAsync(context.GetRouteString("id"), context.RequestAborted)),

                endpoints.MapPost($"{path}/execute/{{id}}", context
                    => context.GetAction()
                              .ExecuteActionAsync(context.GetRouteString("id"), context.RequestAborted))

            };

        private static ActionService GetAction(this HttpContext context)
            => context.RequestServices.GetService<ActionService>();

    }
}
