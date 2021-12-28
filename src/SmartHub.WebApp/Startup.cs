using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartHub.Logic;
using SmartHub.Logic.Data;
using SmartHub.WebApp.Endpoints;

namespace SmartHub.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration["web_insights"]);
            services.AddLogic(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.Use((c, next) => next());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context => context.Response.WriteAsync("Hello form another Device"));
                //endpoints.MapCrud("/mappedDevices", db => db.MappedDevices);

                endpoints.MapScenes("/scenes");
                endpoints.MapDevices("/devices");
                endpoints.MapActions("/actions");
                endpoints.MapEventLogs("/logEvents");

                endpoints.MapGet("/GO", c => c.RequestServices.GetService<SmartThingsClient>()
                                    .ExecuteDeviceAsync("21b89395-01f3-49b4-b284-6530c0022b61", new Models.SmartThings.DeviceExecuteModel
                                    {
                                        Capability = "fanSpeed",
                                        Component = "main",
                                        Command = "value",
                                        Arguments = new object[] { 3 }
                                    }));

            });
        }
    }
}
