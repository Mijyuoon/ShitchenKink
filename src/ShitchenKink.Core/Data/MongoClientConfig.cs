namespace ShitchenKink.Core.Data;

public class MongoClientConfig
{
    public const string Path = "MongoClient";

    public string? Host { get; init; }

    public int Port { get; init; } = 27017;

    public string? Database { get; init; }
}