using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartHub.WebApp.Configs;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var smartThings = new SmartThingsConfig();
            Configuration.Bind("smartthings", smartThings);

            services.AddHttpClient<SmartThingsClient>(client => {
                client.BaseAddress = new Uri("https://api.smartthings.com/v1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smartThings.PAT);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {


                endpoints.MapGet("/scenes", async context => {

                    var data = await context.RequestServices.GetService<SmartThingsClient>()
                                                            .GetScenesAsync();
                    await context.Response.WriteAsJsonAsync(data.Select(s => new { s.SceneId, s.SceneName }));
                });

                endpoints.MapGet("/triggered", async context =>
                {
                    string id = context.Request.Query.First().Value;
                    await context.RequestServices.GetService<SmartThingsClient>()
                                                ._httpClient
                                                .PostAsJsonAsync($"/devices/{id}/commands",
                                                                 new { 
                                                                    commands = new object[] { 
                                                                        new {
                                                                            component = "main",
                                                                            capability = "switch",
                                                                            command = "on"
                                                                        }
                                                                    }
                                                                 });
                });
                endpoints.MapGet("/goScenes", async context => {

                    await context.RequestServices.GetService<SmartThingsClient>()
                                                            .ExeScene(context.Request.Query.First().Value);

                });

                


                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello form another Device");
                });

            });
        }
    }
}
