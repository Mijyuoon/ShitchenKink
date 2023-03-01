using System.Runtime.CompilerServices;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ShitchenKink.Core.Data;
using ShitchenKink.Core.Interfaces;

namespace ShitchenKink.Core.Services;

public class BotService : IHostedService
{
    private readonly DiscordAuthConfig _auth;
    private readonly DiscordSocketClient _client;
    private readonly ILogger<BotService> _logger;

    private readonly IEnumerable<IReadyHandler> _readyHandlers;
    private readonly IEnumerable<IMessageHandler> _messageHandlers;

    public BotService(
        DiscordAuthConfig auth,
        DiscordSocketClient client,
        ILogger<BotService> logger,
        IServiceProvider services)
    {
        _auth = auth;
        _client = client;
        _logger = logger;

        _readyHandlers = services.GetServices<IReadyHandler>();
        _messageHandlers = services.GetServices<IMessageHandler>();

        _client.Log += OnLog;
        _client.Ready += OnReady;
        _client.MessageReceived += OnMessageReceived;
    }

    public async Task StartAsync(CancellationToken _)
    {
        await _client.LoginAsync(_auth.Type, _auth.Token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken _)
    {
        await _client.StopAsync();
    }

    private Task OnLog(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.None,
        };

        _logger.Log(logLevel, message.Exception, "({Source}) {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }

    private async Task OnReady() =>
        await RunEventHandlers(_readyHandlers, eh => eh.OnReadyAsync());

    private async Task OnMessageReceived(SocketMessage message) =>
        await RunEventHandlers(_messageHandlers, eh => eh.OnMessageAsync(message));

    private async Task RunEventHandlers<T>(
        IEnumerable<T> handlers,
        Func<T, Task> callback,
        [CallerMemberName] string source = "?")
    {
        foreach (var handler in handlers)
        {
            try
            {
                await callback(handler);
            }
            catch (Exception ex)
            {
                const string RemovePrefix = "On";

                var eventName = source.StartsWith(RemovePrefix) ? source[RemovePrefix.Length..] : source;
                _logger.LogError(ex, "An unhandled error occurred in a {Source} event handler", eventName);
            }
        }
    }
}