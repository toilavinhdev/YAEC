using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Package.MongoDb.Collections;

public class MongoDbSequence
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public string CollectionName { get; set; } = null!;
    
    public long Value { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatedAt { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ModifiedAt { get; set; }
}