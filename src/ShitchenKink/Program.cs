using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using ShitchenKink.Commands;
using ShitchenKink.Core;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((host, services) =>
    {
        services.AddCoreServices(host.Configuration);
        services.AddCommandServices(host.Configuration);
    })
    .Build();

await host.Services.UseCoreServices();
await host.Services.UseCommandServices();

await host.StartAsync();
await host.WaitForShutdownAsync();