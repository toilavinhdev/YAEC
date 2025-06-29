using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Package.MongoDb.Attributes;

namespace Package.MongoDb.Collections;

public class MongoDbSequence
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public string CollectionName { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public long Value { get; set; }
    
    [BsonUnderscoreElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatedAt { get; set; }
    
    [BsonUnderscoreElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ModifiedAt { get; set; }
}