using Package.MongoDb.Abstractions;
using Package.MongoDb.Attributes;
using YAEC.Shared.Constants.Enumerations.User;

namespace Service.Identity.Domain.Aggregates.UserAggregate;

public class User : AuditableDocument
{
    [BsonUnderscoreElement]
    public string Email { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public bool IsEmailVerified { get; set; }
    
    [BsonUnderscoreElement]
    public string PhoneNumber { get; set; } = null!;
    
    [BsonUnderscoreElement]
    public bool IsPhoneVerified { get; set; }
    
    [BsonUnderscoreElement]
    public UserStatus Status { get; set; }

    [BsonUnderscoreElement]
    public string PasswordHash { get; set; } = null!;

    [BsonUnderscoreElement]
    public List<string> RoleIds { get; set; } = [];

    [BsonUnderscoreElement]
    public List<string> Claims { get; set; } = [];
}