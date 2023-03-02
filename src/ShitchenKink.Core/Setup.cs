using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Extensions;
using ShitchenKink.Core.Interfaces;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Core;

public static class Setup
{
    private const String AuthPath = "Bot:Auth";
    private const string SocketPath = "Bot:Socket";

    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Required Discord configuration
        services.AddConfiguration<DiscordAuthConfig>(configuration, AuthPath);
        services.AddConfiguration<DiscordSocketConfig>(configuration, SocketPath);

        // Discord.Net services
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<CommandService>();

        // Application services
        services.AddSingleton<BotService>();
        services.AddSingleton<BotCommandService>();
        services.AddSingleton<DispatchService>();

        // Hosted application services (in startup order)
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<BotService>());

        // Message event handlers (in call order)
        services.AddSingleton<IMessageHandler>(provider => provider.GetRequiredService<BotCommandService>());
    }

    public static async Task UseCoreServices(this IServiceProvider services)
    {
        // Nothing to do here
        await Task.CompletedTask;
    }
}