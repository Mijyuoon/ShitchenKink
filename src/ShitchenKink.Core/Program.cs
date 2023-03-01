using Microsoft.Extensions.Hosting;

using ShitchenKink.Commands;
using ShitchenKink.Core;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddCoreServices();
        services.AddCommandServices();
    })
    .Build();

await host.Services.UseCoreServices();
await host.Services.UseCommandServices();

await host.StartAsync();
await host.WaitForShutdownAsync();