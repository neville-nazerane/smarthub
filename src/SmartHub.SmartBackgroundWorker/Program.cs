using SmartHub.SmartBackgroundWorker.Services;
using SmartHub.SmartBackgroundWorker.Utils;
using SmartHub.SmartBackgroundWorker.Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService(hostContext.Configuration["smartworker_insights"]);
        services
            //.AddHostedService<SmartWorker>()
            .AddHostedService<MinuteWorker>()
            .AddHostedService<HueWorker>();
        services.AddLogic(hostContext.Configuration)
                .AddScoped<SmartyPants>()
                .AddTransient<HueProcessor>()
                .AddSingleton<MinuiteProcessor>();
    })
    .Build();


await host.RunAsync();
