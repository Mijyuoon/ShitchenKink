using Discord;

namespace ShitchenKink.Core.Data;

public class DiscordAuthConfig
{
    public TokenType Type { get; init; }

    public string? Token { get; init; }
}