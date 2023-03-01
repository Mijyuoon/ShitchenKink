using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;

using ShitchenKink.Commands.Services;

namespace ShitchenKink.Commands;

public static class Setup
{
    public static void AddCommandServices(this IServiceCollection services)
    {
        services.AddSingleton<SpinnerService>();
    }

    public static async Task UseCommandServices(this IServiceProvider services)
    {
        await services.GetRequiredService<CommandService>()
            .AddModulesAsync(typeof(Setup).Assembly, services);
    }
}