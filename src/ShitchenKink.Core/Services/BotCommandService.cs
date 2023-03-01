using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using ShitchenKink.Core.Interfaces;

namespace ShitchenKink.Core.Services;

public class BotCommandService : IMessageHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly ILogger<BotCommandService> _logger;

    // TODO: Move to an external configuration file
    private readonly IEnumerable<string> _prefixes = new[]
    {
        "pls", "go go gadget", "press f to", "ok chiko"
    };

    public BotCommandService(
        DiscordSocketClient client,
        CommandService commands,
        IServiceProvider services,
        ILogger<BotCommandService> logger)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _logger = logger;
    }

    public async Task OnMessageAsync(SocketMessage message)
    {
        // Ignore system messages (e.g. user join notifications)
        if (message is not SocketUserMessage userMessage) return;

        // Ignore messages from bots
        if (message.Author.IsBot) return;

        var commandOffset = -1;

        // Search through the default prefixes
        foreach (var prefix in _prefixes)
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