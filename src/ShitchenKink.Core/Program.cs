using System.Reflection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using ShitchenKink.Commands;
using ShitchenKink.Core.Data;
using ShitchenKink.Core.Interfaces;
using ShitchenKink.Core.Services;

// Configure the dependency injection
void SetupServices(IServiceCollection services)
{
    // Add required configuration
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

    // Add event handlers
    services.AddSingleton<IMessageHandler, BotCommandHandler>();

    // Add command-specific services
    services.SetupCommandServices();
}

// Call required entry points
async Task StartServices(IServiceProvider services)
{
    // Register all commands
    await services.GetRequiredService<CommandService>()
        .AddModulesAsync(Assembly.GetEntryAssembly(), services);

    // Start command-specific services
    await services.StartCommandServices();

    // This must come last as BotService waits forever until stopped
    await services.GetRequiredService<BotService>()
        .StartAsync();
}


var collection = new ServiceCollection();
SetupServices(collection);

var provider = collection.BuildServiceProvider();
await StartServices(provider);