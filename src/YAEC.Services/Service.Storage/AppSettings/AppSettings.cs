using Package.Identity;
using Package.ObjectStorage;
using Package.Redis;
using YAEC.Shared.Extensions;

namespace Service.Storage.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
    
    public RedisOptions Redis { get; set; } = null!;
    
    public ObjectStorageOptions ObjectStorage { get; set; } = null!;
}