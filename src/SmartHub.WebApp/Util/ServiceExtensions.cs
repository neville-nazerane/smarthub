using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartHub.Logic;

namespace SmartHub.WebApp.Util
{
    public static class ServiceExtensions
    {
        public static SmartThingsClient GetSmartThingsClient(this HttpContext context) 
            => context.RequestServices.GetService<SmartThingsClient>();

    }
}
