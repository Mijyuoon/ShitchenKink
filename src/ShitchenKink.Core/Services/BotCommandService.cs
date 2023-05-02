using System.Collections.Concurrent;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Data.Persistent;
using ShitchenKink.Core.Interfaces;

namespace ShitchenKink.Core.Services;

public class BotCommandService : IMessageHandler, IHostedService
{
    private readonly BotCommandConfig _config;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly ILogger<BotCommandService> _logger;
    private readonly IServiceProvider _services;
    private readonly MongoService _mongo;

    private readonly ConcurrentDictionary<ulong, IEnumerable<string>> _userPrefixes = new();

    public BotCommandService(
        BotCommandConfig config,
        DiscordSocketClient client,
        CommandService commands,
        ILogger<BotCommandService> logger,
        IServiceProvider services,
        MongoService mongo)
    {
        _config = config;
        _client = client;
        _commands = commands;
        _logger = logger;
        _services = services;
        _mongo = mongo;
    }

    public IEnumerable<string> DefaultPrefixes => _config.Prefixes;

    public IEnumerable<string> GetUserPrefixes(IUser user)
        => _userPrefixes.GetValueOrDefault(user.Id) ?? Enumerable.Empty<string>();

    public async Task SetUserPrefixesAsync(IUser user, IEnumerable<string> prefixes)
    {
        var prefixesArray = prefixes.ToArray();
        _userPrefixes[user.Id] = prefixesArray;

        var userSettings = _mongo.GetCollection<UserSettingsModel>();

        var filter = Builders<UserSettingsModel>.Filter
            .Eq(m => m.UserId, user.Id);
        var updater = Builders<UserSettingsModel>.Update
            .Set(m => m.CustomPrefixes, prefixesArray);

        await userSettings.UpdateOneAsync(filter, updater, new UpdateOptions { IsUpsert = true });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var userSettings = _mongo.GetCollection<UserSettingsModel>();
        var entries = await userSettings.Find(_ => true).ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            _userPrefixes[entry.UserId] = entry.CustomPrefixes;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }

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

        // Search through custom prefixes for the message author
        if (String.IsNullOrEmpty(commandText) &&
            _userPrefixes.TryGetValue(message.Author.Id, out var userPrefixes))
        {
            foreach (var prefix in userPrefixes)
            {
                if (TryParsePrefix(userMessage, prefix, out commandText)) break;
            }
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