using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Package.MongoDb.Attributes;
using YAEC.Shared.Domain.Abstractions;

namespace Package.MongoDb.Abstractions;

public class AuditableDocument : BaseDocument, IAuditableEntity<string>
{
    [BsonUnderscoreElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatedAt { get; set; }
    
    [BsonUnderscoreElement]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CreatedBy { get; set; }
    
    [BsonUnderscoreElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ModifiedAt { get; set; }
    
    [BsonUnderscoreElement]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ModifiedBy { get; set; }
}