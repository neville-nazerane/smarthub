using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHub.WebApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// SERVICES
var services = builder.Services;

services.AddApplicationInsightsTelemetry(Configuration["web_insights"]);
services.AddCors()
        .AddLogic(Configuration);


var app = builder.Build();

app.UseCors(b => b.AllowAnyHeader()
                              .AllowAnyMethod()
                              .WithOrigins(Configuration["clientUrl"].Split(",")));

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context => context.Response.WriteAsync("Hello form another Device"));

    endpoints.MapBond();
    endpoints.MapScenes();
    endpoints.MapDevices("/devices");
    endpoints.MapActions("/actions");
    endpoints.MapHue("/hue");
    endpoints.MapEventLogs("/logEvents");
    endpoints.MapTriggers("/trigger");

    endpoints.MapCrud("/settings", c => c.Settings);

});

await app.RunAsync();
