using Package.Identity;
using Package.Redis;
using Package.Shared.Extensions;

namespace Service.Catalog;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public RedisOptions RedisOptions { get; set; } = null!;
}