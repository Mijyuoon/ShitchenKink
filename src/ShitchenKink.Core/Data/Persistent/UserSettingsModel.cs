using MongoDB.Bson.Serialization.Attributes;

using ShitchenKink.Core.Services;

namespace ShitchenKink.Core.Data.Persistent;

[MongoCollection("user_settings")]
public class UserSettingsModel
{
    [BsonId]
    public ulong UserId { get; set; }

    [BsonElement("custom_prefixes")]
    public IEnumerable<string> CustomPrefixes { get; set; }
        = Enumerable.Empty<string>();
}