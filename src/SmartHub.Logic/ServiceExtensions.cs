using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartHub.Logic;
using SmartHub.Logic.Automations;
using SmartHub.Logic.Data;
using SmartHub.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddLogic(this IServiceCollection services, IConfiguration configuration)
        {

            var smartThings = new SmartThingsConfig();
            configuration.Bind("smartthings", smartThings);
            services.AddHttpClient<SmartThingsClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.smartthings.com/v1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", smartThings.PAT);
            });


            var backup = new BackUpFunctionConfig();
            configuration.Bind("backup", backup);
            services.AddHttpClient<AzBackupClient>(client =>
            {
                client.BaseAddress = new Uri(backup.BaseUrl);
                client.DefaultRequestHeaders.Add("x-functions-key", backup.Key);
            });

            return services

                        .Configure<GlobalConfig>(configuration.GetSection("global"))

                        .AddDbContext<AppDbContext>((provider, o) => {

                            var options = provider.GetService<IOptions<GlobalConfig>>();
                            
                            string dataPath = options.Value.DataPath
                                                           .TrimEnd('/')
                                                           .TrimEnd('\\');
                            string personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                            o.UseSqlite($"Data Source={Path.Combine(personal, "data", "smartstuffs.db")}", 
                                            b => b.MigrationsAssembly("SmartHub.DbMigrator"));
                            //o.UseMySql(configuration["sql"], ServerVersion.AutoDetect(configuration["sql"]));
                            o.EnableSensitiveDataLogging();
                            //o.LogTo(ShowMe, Logging.LogLevel.Information, DbContextLoggerOptions.SingleLine);
                        })

                        .AddTransient<ActionService>()
                        .AddTransient<EventLogService>()
                        .AddScoped<AutomationService>()
                        .AddTransient<EventService>()
                        
                        //AUTOMATIONS
                        .AddScoped<TurnOnBedroomAutomation>();
        }

        public class SmartThingsConfig
        {

            public string PAT { get; set; }

        }

        public class BackUpFunctionConfig
        {
            public string BaseUrl { get; set; }

            public string Key { get; set; }

        }


    }
}
