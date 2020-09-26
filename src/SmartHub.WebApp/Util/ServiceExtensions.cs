using Microsoft.AspNetCore.Http;
using SmartHub.WebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SmartHub.WebApp.Util
{
    public static class ServiceExtensions
    {
        public static SmartThingsClient GetSmartThingsClient(this HttpContext context) 
            => context.RequestServices.GetService<SmartThingsClient>();

    }
}
