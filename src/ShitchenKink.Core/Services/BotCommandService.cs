using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Interfaces;

namespace ShitchenKink.Core.Services;

public class BotCommandService : IMessageHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly ILogger<BotCommandService> _logger;
    private readonly IServiceProvider _services;

    private readonly BotCommandConfig _commandConfig;

    public BotCommandService(
        DiscordSocketClient client,
        CommandService commands,
        ILogger<BotCommandService> logger,
        IServiceProvider services,
        IConfiguration configuration)
    {
        _client = client;
        _commands = commands;
        _logger = logger;
        _services = services;

        _commandConfig = configuration
            .GetRequiredSection(BotCommandConfig.Path)
            .Get<BotCommandConfig>()!;
    }

    public IEnumerable<string> DefaultPrefixes => _commandConfig.Prefixes;

    public async Task OnMessageAsync(SocketMessage message)
    {
        // Ignore system messages (e.g. user join notifications)
        if (message is not SocketUserMessage userMessage) return;

        // Ignore messages from bots
        if (message.Author.IsBot) return;

        var commandOffset = -1;

        // Search through the default prefixes
        foreach (var prefix in _commandConfig.Prefixes)
        {
            if (userMessage.HasStringPrefix(prefix, ref commandOffset)) break;
        }

        // Exit if not a command message
        if (commandOffset < 0) return;

        // Kludge to ignore the whitespace between the prefix and the command
        while (Char.IsWhiteSpace(userMessage.Content, commandOffset))
        {
            commandOffset += 1;
        }

        _logger.LogInformation(
            "User {User} has used command [{Command}]",
            userMessage.Author.ToString(),
            userMessage.Content[commandOffset..]);

        var context = new SocketCommandContext(_client, userMessage);
        await _commands.ExecuteAsync(context, commandOffset, _services);
    }
}