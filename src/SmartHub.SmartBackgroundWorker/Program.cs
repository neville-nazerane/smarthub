using SmartHub.SmartBackgroundWorker;
using SmartHub.SmartBackgroundWorker.Utils;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService(hostContext.Configuration["smartworker_insights"]);
        services
            .AddHostedService<SmartWorker>()
            .AddHostedService<HueWorker>();
        services.AddLogic(hostContext.Configuration)
                .AddScoped<SmartyPants>();
    })
    .Build();


await host.RunAsync();
