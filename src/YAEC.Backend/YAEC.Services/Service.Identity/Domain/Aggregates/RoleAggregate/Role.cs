using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Package.Shared.Domain;

namespace Service.Identity.Domain.Aggregates.RoleAggregate;

public class Role : IBaseEntity<string>, IAuditableEntity<string>, ISoftDeleteEntity<string>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    public long AutoId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }

    public List<string> Policies { get; set; } = [];
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentId { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatedAt { get; set; }
    
    [MaxLength(128)]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CreatedBy { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ModifiedAt { get; set; }
    
    [MaxLength(128)]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ModifiedBy { get; set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DeletedAt { get; set; }
    
    [MaxLength(128)]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? DeletedBy { get; set; }
}