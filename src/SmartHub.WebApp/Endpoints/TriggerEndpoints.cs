using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SmartHub.Logic;
using SmartHub.WebApp.Util;

namespace SmartHub.WebApp.Endpoints
{
    public static class TriggerEndpoints
    {

        public static IEndpointConventionBuilder MapTriggers(this IEndpointRouteBuilder endpoints, string path)
            => new MultiEndpointConventionBuilder
            {

                endpoints.MapPut($"{path}/fanSpeed/{{fanId}}/{{speed}}", c => 
                                    c.RequestServices.GetService<SmartLogic>().SetFanSpeedAsync(c.GetRouteString("fanId"), c.GetRouteInt("speed"), c.RequestAborted))

            };

    }
}
