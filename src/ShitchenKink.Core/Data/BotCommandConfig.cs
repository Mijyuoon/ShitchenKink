namespace ShitchenKink.Core.Data;

public class BotCommandConfig
{
    public const string Path = "Bot:UserCommands";

    public IEnumerable<string> Prefixes { get; init; }
        = Enumerable.Empty<string>();
}