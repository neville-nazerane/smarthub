using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SmartHub.Logic;
using SmartHub.WebApp.Util;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Endpoints
{
    public static class HueEndpoints
    {

        public static IEndpointConventionBuilder MapHue(this IEndpointRouteBuilder endpoints, string path)
        {
            return new MultiEndpointConventionBuilder
            {
                endpoints.MapGet("/swapme/{state}", StateAction),
                endpoints.MapGet("/hue", StreamAsync)
            };
        }

        static Task StateAction(bool state,
                         HueClient hueClient,
                         CancellationToken cancellationToken = default)
            => hueClient.SwitchLightAsync("404b22ea-8b2f-43ed-93ff-3641f5c478d5", state, cancellationToken);

        static Task<string> StreamAsync(HueClient hueClient, CancellationToken cancellationToken = default)
            => hueClient.StreamEventAsync(cancellationToken);

    }
}
