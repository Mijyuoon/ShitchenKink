using Discord;
using Discord.WebSocket;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Interfaces;

namespace ShitchenKink.Core.Services;

public class BotService
{
    private readonly DiscordAuthConfig _auth;
    private readonly DiscordSocketClient _client;

    private readonly IEnumerable<IMessageHandler> _messageHandlers;

    private TaskCompletionSource? _stopSource;

    public BotService(
        DiscordAuthConfig auth,
        DiscordSocketClient client,
        IEnumerable<IMessageHandler> messageHandlers)
    {
        _auth = auth;
        _client = client;

        _messageHandlers = messageHandlers;

        _client.Log += OnLog;
        _client.MessageReceived += OnMessageReceived;
    }

    public async Task StartAsync()
    {
        if (_stopSource is not null) return;

        await _client.LoginAsync(_auth.Type, _auth.Token);
        await _client.StartAsync();

        _stopSource = new TaskCompletionSource();
        await _stopSource.Task;
    }

    public async Task StopAsync()
    {
        await _client.StopAsync();

        _stopSource?.SetResult();
        _stopSource = null;
    }

    private Task OnLog(LogMessage message)
    {
        // TODO: Use a proper logger
        Console.WriteLine(message.ToString());

        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        foreach (var handler in _messageHandlers)
        {
            await handler.OnMessageAsync(message);
        }
    }
}