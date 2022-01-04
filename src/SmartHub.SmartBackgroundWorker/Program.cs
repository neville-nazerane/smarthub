using SmartHub.SmartBackgroundWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService(hostContext.Configuration["smartworker_insights"]);
        services.AddHostedService<SmartWorker>();
        services.AddLogic(hostContext.Configuration)
                .AddScoped<SmartyPants>();
    })
    .Build();

await host.RunAsync();
