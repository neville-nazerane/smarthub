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
                endpoints.MapPut("bond/fanSpeed/{fanId}/{speed}", SetFanSpeedAsync),
                endpoints.MapGet("bond/fanSpeed/{fanId}", GetFanSpeedAsync),
                endpoints.MapPut("bond/light/{lightId}/{on}", SwitchLightAsync),
                endpoints.MapGet("bond/light/{lightId}", GetLightStateAsync)
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

        static async Task<int> GetFanSpeedAsync(string fanId, BondClient client, CancellationToken cancellationToken = default)
        {
            var status = await client.GetStateAsync(fanId, cancellationToken);
            return status.Power * status.Speed;
        }

        static Task SwitchLightAsync(string lightId, bool on, BondClient client, CancellationToken cancellationToken = default)
            => client.ToggleLightAsync(lightId, on, cancellationToken);

        static async Task<bool> GetLightStateAsync(string lightId, BondClient client, CancellationToken cancellationToken = default)
        {
            var status = await client.GetStateAsync(lightId, cancellationToken);
            return status.Light == 1;
        }

    }
}
