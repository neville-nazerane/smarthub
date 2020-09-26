using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartHub.Models;
using SmartHub.WebApp.Models;
using SmartHub.WebApp.Services;
using SmartHub.WebApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Endpoints
{
    public static class DeviceEndpoints
    {

        public static IEndpointConventionBuilder MapDevices(this IEndpointRouteBuilder endpoints, string path)
        {

            return new MultiEndpointConventionBuilder
            {

                endpoints.MapGet(path, async context => {

                    var devices = await context.GetSmartThingsClient().GetDevicesAsync();
                    await context.Response.WriteAsJsonAsync(devices);

                }),
                endpoints.MapPost($"{path}/{{deviceId}}/execute", async context => {

                    await context.GetSmartThingsClient().ExecuteDeviceAsync(
                                                            context.Request.RouteValues["deviceId"].ToString(),
                                                            await context.Request.ReadFromJsonAsync<DeviceExecuteModel>());
                
                })
                

            };

        }



    }
}
