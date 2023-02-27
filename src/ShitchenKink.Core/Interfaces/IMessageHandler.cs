using Discord.WebSocket;

namespace ShitchenKink.Core.Interfaces;

public interface IMessageHandler
{
    Task OnMessageAsync(SocketMessage message);
}