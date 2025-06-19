using Package.Identity;
using Package.MongoDb;
using Package.Redis;
using YAEC.Shared.Extensions;

namespace Service.Identity.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
    
    public RedisOptions Redis { get; set; } = null!;
    
    public MongoDbOptions MongoDb { get; set; } = null!;
}