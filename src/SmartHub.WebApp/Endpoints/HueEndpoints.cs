using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SmartHub.Constants;
using SmartHub.Logic;
using SmartHub.WebApp.Util;
using System.Net.Http;
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
                endpoints.MapGet("/hue", StreamAsync),

                endpoints.MapGet("/fans/{up}", SwapFanAsync)
            };
        }

        static Task SwapFanAsync(bool up, BondClient bondClient)
        {
            if (up)
                return bondClient.IncreaseFanAsync(DeviceConstants.bondFrontFanId);
            else
                return bondClient.DecreaseFanAsync(DeviceConstants.bondFrontFanId);
        }

        static Task StateAction(bool state,
                         HueClient hueClient,
                         CancellationToken cancellationToken = default)
            => hueClient.SwitchLightAsync("14d8fd8b-f454-4dca-87aa-d9164bbe310c", state, cancellationToken);

        static Task<HttpResponseMessage> StreamAsync(HueClient hueClient, CancellationToken cancellationToken = default)
            => hueClient.StreamEventAsync(cancellationToken);

    }
}
