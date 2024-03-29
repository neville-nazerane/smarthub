using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartHub.Logic;

namespace SmartHub.BackgroundWorker
{
    public class Program
    {
        public static Task Main(string[] args) 
            => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(hostContext.Configuration["worker_insights"]);
                    services.AddHostedService<Worker>()
                            .AddLogic(hostContext.Configuration)
                            .AddScoped<TimeyExecuter>();
                });
    }
}
