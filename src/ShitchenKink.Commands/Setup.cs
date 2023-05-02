using Discord;
using Discord.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ShitchenKink.Commands.Data;
using ShitchenKink.Commands.Readers;
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

    public static async Task StartCommandServices(this IServiceProvider services)
    {
        var commands = services.GetRequiredService<CommandService>();

        //Add IUser reader that resolves non-cached users
        commands.AddTypeReader<IUser>(new ResolveUserReader<IUser>(), true);
        commands.AddTypeReader<IGuildUser>(new ResolveUserReader<IGuildUser>(), true);

        // Register all command modules from this assembly
        await commands.AddModulesAsync(typeof(Setup).Assembly, services);
    }
}