using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Package.MongoDb.Attributes;
using YAEC.Shared.Domain.Abstractions;

namespace Package.MongoDb.Abstractions;

public class BaseDocument : IBaseEntity<string>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public long SubId { get; set; }
}