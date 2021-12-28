using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartHub.Models;
using SmartHub.Models.SmartThings;
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

                endpoints.MapGet(path, async context
                    => await context.Response.WriteAsJsonAsync(
                                                    await context.GetSmartThingsClient()
                                                                 .GetDevicesAsync(
                                                                        context.RequestAborted),
                                                                        context.RequestAborted)),

                endpoints.MapPost($"{path}/{{deviceId}}/execute", async context
                    => await context.GetSmartThingsClient()
                                    .ExecuteDeviceAsync(
                                            context.Request.RouteValues["deviceId"].ToString(),
                                            await context.Request.ReadFromJsonAsync<DeviceExecuteModel>(context.RequestAborted),
                                            context.RequestAborted)),

                endpoints.MapGet($"{path}/{{deviceId}}/components/{{componentId}}/capabilities/{{capabilityId}}/status", async context =>
                        await context.Response.WriteAsJsonAsync(
                                            await context.GetSmartThingsClient()
                                                         .GetCapabilityStatusAsync(
                                                                context.GetRouteString("deviceId"),
                                                                context.GetRouteString("componentId"),
                                                                context.GetRouteString("capabilityId"),
                                                                context.RequestAborted),
                                            context.RequestAborted)),

                endpoints.MapGet($"{path}/capabilities/{{id}}/{{version}}", async context => 
                        await context.Response.WriteAsJsonAsync(
                                            await context.GetSmartThingsClient()
                                                         .GetCapabilityAsync(
                                                                context.GetRouteString("id"), 
                                                                context.GetRouteFloat("version"), 
                                                                context.RequestAborted),
                                            context.RequestAborted))
                

            };

        }



    }
}
