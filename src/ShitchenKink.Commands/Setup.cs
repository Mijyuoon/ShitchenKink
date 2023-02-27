using Microsoft.Extensions.DependencyInjection;

using ShitchenKink.Commands.Services;

namespace ShitchenKink.Commands;

public static class Setup
{
    public static void SetupCommandServices(this IServiceCollection services)
    {
        services.AddSingleton<SpinnerService>();
    }

    public static Task StartCommandServices(this IServiceProvider services)
    {
        return Task.CompletedTask;
    }
}