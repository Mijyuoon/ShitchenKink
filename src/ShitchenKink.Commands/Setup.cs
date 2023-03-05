using Discord.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ShitchenKink.Commands.Data;
using ShitchenKink.Commands.Services;
using ShitchenKink.Core.Extensions;

namespace ShitchenKink.Commands;

public static class Setup
{
    public static void AddCommandServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Static command configuration
        services.AddConfiguration<SpinnerConfig>(configuration, SpinnerConfig.Path);

        // Services used by commands
        services.AddSingleton<SpinnerService>();
        services.AddSingleton<IfunnyService>();
    }

    public static async Task UseCommandServices(this IServiceProvider services)
    {
        // Register all command modules from this assembly
        await services.GetRequiredService<CommandService>()
            .AddModulesAsync(typeof(Setup).Assembly, services);
    }
}