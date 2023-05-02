using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using ShitchenKink.Commands;
using ShitchenKink.Core;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, config) =>
    {
        config.AddJsonFile("appsettings.json");

        var environment = host.HostingEnvironment.EnvironmentName;
        config.AddJsonFile($"appsettings.{environment}.json", optional: true);

        config.AddEnvironmentVariables();
    })
    .ConfigureServices((host, services) =>
    {
        services.AddCoreServices(host.Configuration);
        services.AddCommandServices(host.Configuration);
    })
    .Build();

await host.Services.StartCoreServices();
await host.Services.StartCommandServices();

await host.StartAsync();
await host.WaitForShutdownAsync();