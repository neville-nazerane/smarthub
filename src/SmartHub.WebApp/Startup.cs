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
using SmartHub.WebApp.Configs;
using SmartHub.WebApp.Data;
using SmartHub.WebApp.Endpoints;
using SmartHub.WebApp.Services;

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
            var smartThings = new SmartThingsConfig();
            Configuration.Bind("smartthings", smartThings);

            services.AddHttpClient<SmartThingsClient>(client => {
                client.BaseAddress = new Uri("https://api.smartthings.com/v1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smartThings.PAT);
            });

            services.AddDbContext<AppDbContext>(o => o.UseMySql(Configuration["sql"], ServerVersion.AutoDetect(Configuration["sql"])));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapGet("/", context => context.Response.WriteAsync("Hello form another Device"));
                endpoints.MapCrud("/mappedDevices", db => db.MappedDevices);
                endpoints.MapDevices("/devices");

            });
        }
    }
}
