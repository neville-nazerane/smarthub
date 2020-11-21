using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartHub.Logic;
using SmartHub.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddLogic(this IServiceCollection services, IConfiguration configuration)
        {

            var smartThings = new SmartThingsConfig();
            configuration.Bind("smartthings", smartThings);

            services.AddHttpClient<SmartThingsClient>(client => {
                client.BaseAddress = new Uri("https://api.smartthings.com/v1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smartThings.PAT);
            });

            return services

                        .AddDbContext<AppDbContext>(o => o.UseMySql(configuration["sql"], ServerVersion.AutoDetect(configuration["sql"])))

                        .AddTransient<ActionService>();
        }

        public class SmartThingsConfig
        {

            public string PAT { get; set; }

        }

    }
}
