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

    private readonly BotCommandConfig _config;

    public BotCommandService(
        DiscordSocketClient client,
        CommandService commands,
        ILogger<BotCommandService> logger,
        IServiceProvider services,
        BotCommandConfig config)
    {
        _client = client;
        _commands = commands;
        _logger = logger;
        _services = services;
        _config = config;
    }

    public IEnumerable<string> DefaultPrefixes => _config.Prefixes;

    public async Task OnMessageAsync(SocketMessage message)
    {
        // Ignore system messages (e.g. user join notifications)
        if (message is not SocketUserMessage userMessage) return;

        // Ignore messages from bots
        if (message.Author.IsBot) return;

        var commandText = String.Empty;

        // Search through the default prefixes
        foreach (var prefix in _config.Prefixes)
        {
            if (TryParsePrefix(userMessage, prefix, out commandText)) break;
        }

        // Exit if not a command message
        if (String.IsNullOrWhiteSpace(commandText)) return;

        _logger.LogInformation("User {User} has used command [{Command}]", userMessage.Author, commandText);

        var context = new SocketCommandContext(_client, userMessage);
        await _commands.ExecuteAsync(context, commandText, _services);
    }

    private static bool TryParsePrefix(SocketUserMessage message, string prefix, out string command)
    {
        var input = message.Content;

        if (input.StartsWith(prefix))
        {
            command = input[prefix.Length..].Trim();
            return true;
        }

        command = String.Empty;
        return false;
    }
}