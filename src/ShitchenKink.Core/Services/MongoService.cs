using System.Reflection;

using MongoDB.Driver;

using ShitchenKink.Core.Data;

namespace ShitchenKink.Core.Services;

public class MongoService
{
    private readonly MongoClient _client;
    private readonly MongoClientConfig _config;

    public MongoService(MongoClient client, MongoClientConfig config)
    {
        _client = client;
        _config = config;
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        var database = _client.GetDatabase(_config.Database);
        var collection = GetCollectionNameForType(typeof(T));

        return database.GetCollection<T>(collection);
    }

    private string GetCollectionNameForType(Type type)
    {
        if (type.GetCustomAttribute<MongoCollectionAttribute>() is { Name: var collectionName })
        {
            return collectionName;
        }

        return type.Name;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class MongoCollectionAttribute : Attribute
{
    public string Name { get; }

    public MongoCollectionAttribute(string name)
    {
        Name = name;
    }
}