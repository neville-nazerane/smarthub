using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Util
{
    public static class HttpContextExtensions
    {

        public static int GetRouteInt(this HttpContext context, string key)
            => int.Parse(context.GetRouteString(key));

        public static float GetRouteFloat(this HttpContext context, string key)
            => float.Parse(context.GetRouteString(key));
        public static string GetRouteString(this HttpContext context, string key)
            => context.Request.RouteValues[key].ToString();

    }
}
