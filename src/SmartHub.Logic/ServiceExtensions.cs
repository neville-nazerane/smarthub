﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using SmartHub.Logic;
using SmartHub.Logic.Automations;
using SmartHub.Logic.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                        .AddDbContext<AppDbContext>(o => {
                            o.UseMySql(configuration["sql"], ServerVersion.AutoDetect(configuration["sql"]));
                            o.EnableSensitiveDataLogging();
                            o.LogTo(ShowMe, Logging.LogLevel.Information, DbContextLoggerOptions.SingleLine);
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

        public  static void ShowMe(string str)
        {

        }

    }
}
