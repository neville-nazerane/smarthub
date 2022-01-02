using SmartHub.SmartBackgroundWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<SmartWorker>();
        services.AddLogic(hostContext.Configuration);
    })
    .Build();

await host.RunAsync();
