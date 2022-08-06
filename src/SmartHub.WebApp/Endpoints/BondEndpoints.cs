using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SmartHub.Logic;
using SmartHub.WebApp.Util;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Endpoints
{
    public static class BondEndpoints
    {
        public static IEndpointConventionBuilder MapBond(this IEndpointRouteBuilder endpoints)
        {
            return new MultiEndpointConventionBuilder
            {
                endpoints.MapPut("bond/fanSpeed/{fanId}/{speed}", SetFanSpeedAsync)
            };
        }

        static async Task SetFanSpeedAsync(string fanId,
                                    int speed,
                                    BondClient client,
                                    CancellationToken cancellationToken = default)
        {
            await client.SetFanSpeedAsync(fanId, speed, cancellationToken);
            var state = await client.GetStateAsync(fanId, cancellationToken);
            if (state.Power == 0 && speed > 0)
                await client.ToggleFanAsync(fanId, true, cancellationToken);
            else if (state.Power == 1 && speed == 0)
                await client.ToggleFanAsync(fanId, false, cancellationToken);
        }

    }
}
