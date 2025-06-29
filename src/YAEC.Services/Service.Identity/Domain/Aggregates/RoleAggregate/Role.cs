using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Package.MongoDb.Abstractions;
using Package.MongoDb.Attributes;

namespace Service.Identity.Domain.Aggregates.RoleAggregate;

public class Role : AuditableDocument
{
    [BsonUnderscoreElement]
    public string DisplayName { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public string NormalizedName { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public string? Description { get; set; }
    
    [BsonUnderscoreElement]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentId { get; set; }

    [BsonUnderscoreElement]
    public List<string> Claims { get; set; } = [];
}