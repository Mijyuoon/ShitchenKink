namespace ShitchenKink.Core.Data;

public class BotCommandConfig
{
    public const string Path = "Commands";

    public IEnumerable<string> Prefixes { get; init; }
        = Enumerable.Empty<string>();
}