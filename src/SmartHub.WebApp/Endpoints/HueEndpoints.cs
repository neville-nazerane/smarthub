using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SmartHub.Constants;
using SmartHub.Logic;
using SmartHub.WebApp.Util;
using System.Linq;
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
                endpoints.MapGet("/hue", StreamAsync),


                endpoints.MapGet("/hue/light/{id}", GetLightOnAsync),
                endpoints.MapPut("/hue/light/{id}/{switchOn}", SwitchLightAsync)

            };
        }

        static Task<HttpResponseMessage> StreamAsync(HueClient hueClient, CancellationToken cancellationToken = default)
            => hueClient.StreamEventAsync(cancellationToken);

        static async Task<bool> GetLightOnAsync(HueClient hueClient,
                                    string id,
                                    CancellationToken cancellationToken = default)
            => (await hueClient.GetLightInfoAsync(id, cancellationToken)).Data.First().On.On;

        static Task SwitchLightAsync(HueClient hueClient,
                                    string id,
                                    bool switchOn,
                                    CancellationToken cancellationToken = default)
            => hueClient.SwitchLightAsync(id, switchOn, cancellationToken);

    }
}
