﻿using Microsoft.AspNetCore.Http;
using SmartHub.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartHub.WebApp.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SmartHub.WebApp.Endpoints
{
    public static class EventLogEndpoints
    {

        public static IEndpointConventionBuilder MapEventLogs(this IEndpointRouteBuilder endpoints, string path)
            => new MultiEndpointConventionBuilder 
            { 
                
                endpoints.MapGet($"{path}/add/{{event}}", context 
                    => context.GetLogService().LogAsync(context.GetRouteString("event"), context.RequestAborted))

            };

        private static EventLogService GetLogService(this HttpContext context)
            => context.RequestServices.GetService<EventLogService>();

    }
}