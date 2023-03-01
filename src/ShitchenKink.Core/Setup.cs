using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Interfaces;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Core;

public static class Setup
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        // Set up required Discord configuration
        services.AddSingleton(new DiscordAuthConfig
        {
            Type = TokenType.Bot,
            Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")!,
        });

        services.AddSingleton(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
        });

        // Add Discord.Net services
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();

        // Add application services
        services.AddSingleton<BotService>();
        services.AddSingleton<BotCommandService>();
        services.AddSingleton<DispatchService>();

        // Add hosted services (in startup order)
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<BotService>());

        // Add event handlers (in call order)
        services.AddSingleton<IMessageHandler>(provider => provider.GetRequiredService<BotCommandService>());
    }

    public static async Task UseCoreServices(this IServiceProvider services)
    {
        // Nothing to do here
        await Task.CompletedTask;
    }
}